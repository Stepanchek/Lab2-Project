using System.Xml.Serialization;
using System.Collections.Generic;

namespace PartyQuiz.Siq
{
    [XmlRoot(ElementName = "authors")]
    public class Authors
    {
        [XmlElement(ElementName = "author")]
        public string Author { get; set; }
    }

    [XmlRoot(ElementName = "info")]
    public class Info
    {
        [XmlElement(ElementName = "authors")]
        public Authors Authors { get; set; }
    }

    [XmlRoot(ElementName = "scenario")]
    public class Scenario
    {
        [XmlElement(ElementName = "atom")]
        public List<Atom> Atom { get; set; }
    }

    [XmlRoot(ElementName = "right")]
    public class Right
    {
        [XmlElement(ElementName = "answer")]
        public string Answer { get; set; }
    }

    [XmlRoot(ElementName = "question")]
    public class Question
    {
        [XmlElement(ElementName = "scenario")]
        public Scenario Scenario { get; set; }

        [XmlElement(ElementName = "right")]
        public Right Right { get; set; }

        [XmlAttribute(AttributeName = "price")]
        public string Price { get; set; }

        [XmlElement(ElementName = "type")]
        public Type Type { get; set; }
    }

    [XmlRoot(ElementName = "questions")]
    public class Questions
    {
        [XmlElement(ElementName = "question")]
        public List<Question> Question { get; set; }
    }

    [XmlRoot(ElementName = "theme")]
    public class Theme
    {
        [XmlElement(ElementName = "questions")]
        public Questions Questions { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
    }

    [XmlRoot(ElementName = "param")]
    public class Param
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlText] public string Text { get; set; }
    }

    [XmlRoot(ElementName = "type")]
    public class Type
    {
        [XmlElement(ElementName = "param")]
        public List<Param> Param { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
    }

    [XmlRoot(ElementName = "atom")]
    public class Atom
    {
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }

        [XmlText]
        public string Text { get; set; }

        [XmlAttribute(AttributeName = "time")]
        public string Time { get; set; }
    }

    [XmlRoot(ElementName = "themes")]
    public class Themes
    {
        [XmlElement(ElementName = "theme")]
        public List<Theme> Theme { get; set; }
    }

    [XmlRoot(ElementName = "round")]
    public class Round
    {
        [XmlElement(ElementName = "info")]
        public Info Info { get; set; }

        [XmlElement(ElementName = "themes")]
        public Themes Themes { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
    }

    [XmlRoot(ElementName = "rounds")]
    public class Rounds
    {
        [XmlElement(ElementName = "round")]
        public List<Round> Round { get; set; }
    }

    [XmlRoot(ElementName = "package")]
    public class SiqData
    {
        [XmlElement(ElementName = "info")]
        public Info Info { get; set; }

        [XmlElement(ElementName = "rounds")]
        public Rounds Rounds { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }

        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "date")]
        public string Date { get; set; }

        [XmlAttribute(AttributeName = "publisher")]
        public string Publisher { get; set; }

        [XmlAttribute(AttributeName = "difficulty")]
        public string Difficulty { get; set; }

        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }
}