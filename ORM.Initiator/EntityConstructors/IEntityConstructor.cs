namespace ORM.Initiator.EntityConstructors
{
    public interface IEntityConstructor
    {
        void BeginConstructAndSaveEntity();
        void ConstructAndSaveEntity(Classes.Elements.EntityElement entityElement);
        void EndConstructAndSaveEntity();
        void AddReference(Classes.Elements.LoadElement loadElement);
    }
}