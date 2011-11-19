using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace APIMappingGenerator
{
    [Serializable]
    [XmlRoot("doc")]
    public class XmlDoc
    {
        private Collection<XmlDocMember> members = new Collection<XmlDocMember>();

        [XmlArray("members")]
        [XmlArrayItem("member")]
        public Collection<XmlDocMember> Members
        {
            get
            {
                return members;
            }
        }

    }

    [Serializable]
    [DebuggerDisplay("{Name}")]
    public class XmlDocMember
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        public MemberType MemberType
        {
            get
            {
                if (Name.StartsWith("T:"))
                    return MemberType.Class;
                else if (Name.StartsWith("M:"))
                    return MemberType.Method;
                else if (Name.StartsWith("P:"))
                    return MemberType.Property;
                else if (Name.StartsWith("F:"))
                    return MemberType.Field;
                else if (Name.StartsWith("E:"))
                    return MemberType.Event;
                else
                    throw new Exception("Unnown member type");
            }
        }

        [XmlElement("summary")]
        public XmlDocMemberSummary Summary { get; set; }

        [XmlElement("returns")]
        public string Returns { get; set; }

        private Collection<XmlDocMemberParam> items = new Collection<XmlDocMemberParam>();

        [XmlElement("param")]
        public Collection<XmlDocMemberParam> Params
        {
            get
            {
                return items;
            }
        }

        [XmlElement("remarks")]
        public string Remarks { get; set; }

        [XmlElement("example")]
        public string Example { get; set; }

    }

    [Serializable]
    public class XmlDocMemberSummary
    {
        [XmlText]
        public string Summary { get; set; }

        [XmlElement("remarks")]
        public string Remarks { get; set; }

    }

    [Serializable]
    [DebuggerDisplay("Param {Name}")]
    public class XmlDocMemberParam
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlText]
        public string ParamComments { get; set; }
    }

    public enum MemberType
    {
        Unknow,
        Class, // T
        Method, // M
        Property, // P
        Field, // F
        Event, // E
    }

}
