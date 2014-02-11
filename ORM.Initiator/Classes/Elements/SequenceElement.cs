using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using ORM.Initiator.EntityConstructors;

namespace ORM.Initiator.Classes.Elements
{
    [DebuggerDisplay("<{ElementName} name={Name}>")]
    public class SequenceElement : ContainerElement
    {
        public const string NodeName = Constants.Tags.SEQUENCE_NODE;
        private string _name;
        public const int DefaultMin = 0;
        public const int DefaultMax = 1;
        public const int DefaultStep = 1;
        public const int ShowProgressPercent = 10;
        public const double ShowProgressIntervalSec = 5;
        private int _min = DefaultMin;
        private int _max = DefaultMax;
        private int _step = DefaultStep;
        private bool _showProgress = true;

        public SequenceElement(string name)
        {
            Name = name;
        }

        public SequenceElement(RootConfigElement rootConfig, XElement xmlElement)
            : base(rootConfig, xmlElement)
        {
        }

        /// <summary>
        /// Node name in the xml documents
        /// </summary>
        public override string ElementName
        {
            get { return NodeName; }
        }

        /// <summary>
        /// Identification name
        /// </summary>
        public string Name
        {
            get { return _name; }
            protected internal set
            {
                _name = value ?? "sequence";
                InnerXElement.SetAttributeValue(Constants.Tags.NAME_TAG, value);
            }
        }

        /// <summary>
        /// The initial value of the sequence
        /// </summary>
        public int Min
        {
            get { return _min; }
            protected internal set
            {
                _min = value;
                InnerXElement.SetAttributeValue(Constants.Tags.MIN_TAG, value);
            }
        }

        /// <summary>
        /// The maximum value of the sequence (inclusive)
        /// </summary>
        public int Max
        {
            get { return _max; }
            protected internal set
            {
                _max = value;
                InnerXElement.SetAttributeValue(Constants.Tags.MAX_TAG, value);
            }
        }

        /// <summary>
        /// Step of the sequence
        /// </summary>
        public int Step
        {
            get { return _step; }
            protected internal set
            {
                _step = value;
                InnerXElement.SetAttributeValue(Constants.Tags.STEP_TAG, value);
            }
        }

        /// <summary>
        /// Show progress?
        /// </summary>
        public bool ShowProgress
        {
            get { return _showProgress; }
            protected internal set
            {
                _showProgress = value;
                InnerXElement.SetAttributeValue(Constants.Tags.PROGRESS_TAG, value);
            }
        }

        /// <summary>
        /// Current value of sequence
        /// </summary>
        public int CurrentValue { get; private set; }

        protected override void InnerInitialize(XElement xelement)
        {
            // attribute 'name'
            var attribute = xelement.Attribute(Constants.Tags.NAME_TAG);
            Name = attribute == null ? null : attribute.Value;
            // attribute 'min'
            attribute = xelement.Attribute(Constants.Tags.MIN_TAG);
            Min = attribute == null ? DefaultMin : int.Parse(attribute.Value);
            // attribute 'max'
            attribute = xelement.Attribute(Constants.Tags.MAX_TAG);
            Max = attribute == null ? Min : int.Parse(attribute.Value);
            // attribute 'step'
            attribute = xelement.Attribute(Constants.Tags.STEP_TAG);
            Step = attribute == null ? DefaultStep : int.Parse(attribute.Value);
            // attribute 'progress'
            attribute = xelement.Attribute(Constants.Tags.PROGRESS_TAG);
            ShowProgress = attribute == null ? _showProgress : Boolean.Parse(attribute.Value);

            // backup inner data
            BackupInnerXElement();
        }

        protected internal override void Execute()
        {
            // execute the child elements
            // in the loop
            int lastShowPercent = 0;
            var sw = new Stopwatch();
            sw.Start();
            for (int i = Min; i <= Max; i += Step)
            {
                int doneSeq = Convert.ToInt32(i/(double) Max*100);
                if (ShowProgress && (doneSeq % ShowProgressPercent == 0 || sw.Elapsed.TotalSeconds >= ShowProgressIntervalSec)
                    && lastShowPercent != doneSeq)
                {
                    lastShowPercent = doneSeq;
                    sw.Restart();
                    Console.WriteLine("Sequence '{0}' progress: {1}%", Name, doneSeq);
                }
                // substitute inner xml data
                InnerXElement = XElement.Parse(SubstituteInnerXml(i, BakInnerXElement.ToString()));
                // create children elements
                base.InnerInitialize(InnerXElement);
                // enumerate child elements
                foreach (BaseElement element in Elements)
                {
                    // transmit entity constructor to children
                    element.EntityConstructor = EntityConstructor;
                    // and execute process for the entity element
                    var entityElement = element as EntityElement;
                    if (entityElement != null)
                        EntityConstructor.ConstructAndSaveEntity(entityElement);
                    else
                        element.Execute();
                }
            }
        }

        // Regex for default name counter
        private static readonly Regex ExtractDefName = new Regex(@"(\[\$sequence\])", RegexOptions.Compiled | RegexOptions.Multiline);
        // Regex for function
        private static readonly Regex ExtractFunc = new Regex(@"\[#(?<name>[a-zA-Z_]+)(?<mask>[0-9#]*(\.[0-9#]+)?)\]", RegexOptions.Compiled | RegexOptions.Multiline);

        private string SubstituteInnerXml(int counter, string innerXml)
        {
            // replace default name counter
            string resultXml = ExtractDefName.Replace(innerXml, counter.ToString());
            // replace the named counter
            resultXml = Regex.Replace(resultXml, String.Format(@"(\[\${0}\])", Name), counter.ToString(), RegexOptions.Multiline);
            // replace functions
            return ExtractFunc.Replace(resultXml, new MatchEvaluator(ComputeReplacement));
        }

        // You can vary the replacement text for each match on-the-fly
        private String ComputeReplacement(Match m)
        {
            // get function name
            var groupName = m.Groups["name"];
            var groupMaskValue = m.Groups["mask"].Value;
            switch (groupName.Value)
            {
                // random number
                case "random":
                case "Random":
                    string mask = String.IsNullOrEmpty(groupMaskValue) ? "10" : groupMaskValue;
                    return TextRoutine.GetMaskNumber(mask);
                // lower case word
                case "randomword":
                    return String.IsNullOrEmpty(groupMaskValue) ? TextRoutine.GetVariousWord() : TextRoutine.GetVariousWord(int.Parse(groupMaskValue));
                // title case word
                case "RandomWord":
                case "Randomword":
                    string randWord = String.IsNullOrEmpty(groupMaskValue) ? TextRoutine.GetVariousWord() : TextRoutine.GetVariousWord(int.Parse(groupMaskValue));
                    //return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(randWord);
                    return Char.ToUpper(randWord[0]) + randWord.Substring(1);
                default:
                    throw new ParseException("[SequenceElement] Unknown function name: " + groupName.Value);
            }
        }
    }
}
