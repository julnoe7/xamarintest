using System.Runtime.Serialization;

namespace PassbookTicketGenerator.Models {

    [DataContract(Name = "PassField")]
    public class PassField
    {
        public PassField(string key, string label, string value) {
            this.Key = key;
            this.Label = label;
            this.Value = value;
        }

        [DataMember(Name = "key")]
        public string Key { get; set; }
        [DataMember(Name = "label")]
        public string Label { get; set; }
        [DataMember(Name = "value")]
        public string Value { get; set; }
        [DataMember(Name = "textAlignment", EmitDefaultValue = false)]
        public string TextAlign { get; set; }
        [DataMember(Name = "dateStyle", EmitDefaultValue = false)]
        public string DateStyle { get; set; }
        [DataMember(Name = "timeStyle", EmitDefaultValue = false)]
        public string TimeStyle { get; set; }
        [DataMember(Name = "semantics", EmitDefaultValue = false)]
        public SemanticsValue Semantics { get; set; }
    }

    public class SemanticsValue {
        [DataMember(Name = "eventStartDate", EmitDefaultValue = false)]
        public string EventStartDate { get; set; }
        [DataMember(Name = "eventEndDate", EmitDefaultValue = false)]
        public string EventEndDate { get; set; }
    }
}