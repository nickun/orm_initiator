using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using NHibernate;
using ORM.Initiator.Classes;
using ORM.Initiator.Classes.Elements;
using ORM.Initiator.Helpers;

namespace ORM.Initiator.EntityConstructors
{
    public class HibernateEntityConstructor : IEntityConstructor
    {
        private readonly ISessionFactory _factory;
        private ISession _session;
        //private ITransaction _transaction;
        private readonly Dictionary<string, object> _references;
        private int _openSessionCnt;

        private struct StoreEntity
        {
            public bool CanSave;
            public bool Stored;
            public object Entity;
            public string RefName;

            public StoreEntity(bool stored, object entity, string refName)
            {
                CanSave = true;
                Stored = stored;
                Entity = entity;
                RefName = refName;
            }
        }

        public HibernateEntityConstructor(ISessionFactory factory)
        {
            _factory = factory;
            _references = new Dictionary<string, object>();
        }

        public void BeginConstructAndSaveEntity()
        {
            System.Threading.Interlocked.Increment(ref _openSessionCnt);
            if (_session == null)
            {
                _session = _factory.OpenSession();
                //_transaction = _session.BeginTransaction();
            }
        }

        public void EndConstructAndSaveEntity()
        {
            System.Threading.Interlocked.Decrement(ref _openSessionCnt);
            if (System.Threading.Interlocked.CompareExchange(ref _openSessionCnt, 0, -1) == 0)
                if (_session != null)
                {
                    //_transaction.Commit();
                    //_transaction.Dispose();
                    _session.Close();
                    _session = null;
                }
        }

        public void ConstructAndSaveEntity(EntityElement entityElement)
        {
            StoreEntity storeEntityObject;
            try
            {
                storeEntityObject = InternalConstructEntity(entityElement);
            }
            catch (Exception ex)
            {
                throw new ConstructException(ex.Message, ex);
            }

            if (storeEntityObject.Entity == null)
                throw new ConstructException("Cannot create entity, unknown error.");

            if (storeEntityObject.CanSave)
            {
                ISession session = _session;
                bool needCloseSession = false;
                if (session == null)
                {
                    session = _factory.OpenSession();
                    needCloseSession = true;
                }

                try
                {
                    if (storeEntityObject.Stored)
                        session.Update(storeEntityObject.Entity);
                    else
                        session.Save(storeEntityObject.Entity);
                    session.Flush();
                }
                catch (Exception ex)
                {
                    var className = storeEntityObject.Entity.GetType().Name;
                    throw new ConstructException(
                        String.Format("[{0}] {1}",
                            className +
                            (String.IsNullOrWhiteSpace(storeEntityObject.RefName)
                                ? ""
                                : " -> " + storeEntityObject.RefName),
                            ex.Message),
                        ex);
                }
                finally
                {
                    if (needCloseSession)
                        session.Close();
                }
            }
        }

        private StoreEntity InternalConstructEntity(EntityElement entityElement)
        {
            Type cType = TypeResolver.FindType(entityElement.RootConfig.Assembly,
                entityElement.RootConfig.Namespace + "." + entityElement.ClassName);
            if (cType == null)
                throw new ConstructException(String.Format("Type of '{0}' not found.",
                    entityElement.RootConfig.Namespace + "." + entityElement.ClassName));

            // create instance of type cType or get from reference dictionary
            StoreEntity storeEntityObject = !String.IsNullOrEmpty(entityElement.RefName) &&
                                            _references.ContainsKey(entityElement.RefName)
                ? new StoreEntity(true, _references[entityElement.RefName], entityElement.RefName)
                : new StoreEntity(false, Activator.CreateInstance(cType), entityElement.RefName);
            // check flag 'save'
            if (!entityElement.Save)
            {
                // do not need to save entity!
                storeEntityObject.CanSave = false;
                // set id to empty
                var idPropInfo = cType.GetProperty("Id");
                if (idPropInfo != null)
                    idPropInfo.SetValue(storeEntityObject.Entity, Guid.Empty, null);
            }
            // check for similar types
            if (storeEntityObject.Entity.GetType() != cType)
                throw new ConstructException(String.Format("Previously created instance of the same name '{0}' has a different type ({1}) than that ({2}).",
                            entityElement.RefName, storeEntityObject.Entity.GetType().Name, cType.Name));
            // store to reference dictionary
            if (!String.IsNullOrEmpty(entityElement.RefName))
                _references[entityElement.RefName] = storeEntityObject.Entity;
            // set properties
            foreach (EntityProperty property in entityElement.Properties)
            {
                if (property.ValueType == EntityProperty.ValueTypes.Reference)
                {
                    string subRefProperty = ""; // sub reference property name
                    // if the specified property of the entity reference then extract property name
                    if (property.Value.Contains("."))
                    {
                        string[] refParts = property.Value.Split(new[] {'.'}, 2);
                        property.Value = refParts[0];   // correct reference name
                        subRefProperty = refParts[1];   // property of reference
                    }
                    if (!_references.ContainsKey(property.Value))
                        throw new ConstructException(String.Format( "[{0}.{1}] The entity reference name was not found: {2}",
                            entityElement.ClassName, property.Name, property.Value));

                    if (String.IsNullOrEmpty(subRefProperty))
                        property.ObjectValue = _references[property.Value];
                    else
                    {
                        // get property Info from reference
                        var subRefPropInfo = _references[property.Value].GetType().GetProperty(subRefProperty);
                        if (subRefPropInfo == null)
                            throw new ConstructException(
                                String.Format("[{0}.{1}] The property of the entity reference name was not found: {2}.{3}",
                                    entityElement.ClassName, property.Name, property.Value, subRefProperty));
                        // get property Value from reference and remember
                        property.ObjectValue = subRefPropInfo.GetValue(_references[property.Value], null);
                    }
                }

                // find a property in our entity
                var propInfo = cType.GetProperty(property.Name);
                try
                {
                    if (propInfo == null)
                        throw new ConstructException(
                            String.Format("[{0}.{1}] The property of the entity was not found: {2}",
                                entityElement.ClassName, property.Name, property.Name));

                    object oldPropValue = propInfo.GetValue(storeEntityObject.Entity, null);
                    // if old value of the property is not null and is IList
                    if (oldPropValue is IList)
                    {
                        IList pList = (IList)oldPropValue;
                        pList.Add(property.ObjectValue);
                    }
                    // else if generic type like ICollection<T>
                    else if (oldPropValue != null && oldPropValue.GetType().IsGenericType && oldPropValue is IEnumerable)
                    {
                        var methodAdd = oldPropValue.GetType().GetMethod("Add", BindingFlags.Instance | BindingFlags.Public);
                        if (methodAdd == null)
                            throw new ConstructException(
                                String.Format("[{0}.{1}] The collection does not have 'Add' method: {2}",
                                    entityElement.ClassName, property.Name, property.Name));
                        methodAdd.Invoke(oldPropValue, new[] { property.ObjectValue });
                    }
                    else if (oldPropValue == null && propInfo.PropertyType.IsGenericType && typeof(IEnumerable).IsAssignableFrom(propInfo.PropertyType))
                    {
                        throw new ConstructException(
                                String.Format("[{0}.{1}] The collection can not be null.",
                                entityElement.ClassName, property.Name));
                    }
                    else
                        // else set the value to the property of the object
                        propInfo.SetValue(storeEntityObject.Entity, property.ObjectValue, null);
                }
                catch (Exception ex)
                {
                    throw new ConstructException(String.Format("Can not set the value '{0}' to the property '{1}'.\r\n{2}\r\nSee element:\r\n{3}",
                        property.Value, property.Name, ex.Message, entityElement.ToXmlString()), ex);
                }
            }

            return storeEntityObject;
        }

        public void AddReference(LoadElement loadElement)
        {
            if (!_references.ContainsKey(loadElement.RefName))
            {
                IList objList = _session.CreateQuery(loadElement.Where).List();
                if (objList.Count > 0)
                {
                    _references.Add(loadElement.RefName, objList[0]);
                }
            }
        }
    }
}