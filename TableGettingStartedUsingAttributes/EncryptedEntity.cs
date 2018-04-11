namespace TableGettingStartedUsingAttributes
{
    using Microsoft.WindowsAzure.Storage.Table;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class EncryptedEntity : TableEntity
    {
        public EncryptedEntity()
        {
        }

        public EncryptedEntity(string pk, string rk)
            : base(pk, rk)
        {
        }

        public void Populate()
        {
            this.EncryptedProperty1 = string.Empty;
            this.EncryptedProperty2 = "foo";
            this.NotEncryptedProperty = "b";
            this.NotEncryptedIntProperty = 1234;
        }

        [EncryptProperty]
        public string EncryptedProperty1 { get; set; }

        [EncryptProperty]
        public string EncryptedProperty2 { get; set; }

        public string NotEncryptedProperty { get; set; }
    
        public int NotEncryptedIntProperty { get; set; }
    }
}
