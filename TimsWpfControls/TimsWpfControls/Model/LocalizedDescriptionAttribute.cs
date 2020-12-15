using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace TimsWpfControls.Model
{


    public class LocalizedDescriptionAttribute : DescriptionAttribute
    {
        private readonly ResourceManager resourceManager;
        private readonly string resourceKey;

        /// <summary>
        /// Creates a new LocalizedDescription-Attribute
        /// </summary>
        /// <param name="resourceKey">the Key of the the resourcestring to lookup</param>
        /// <param name="resourceType">the type of the ResourceDictionary where the data is stored</param>
        public LocalizedDescriptionAttribute(string resourceKey, Type resourceType)
        {
            this.resourceKey = resourceKey;
            this.resourceManager = new ResourceManager(resourceType);
        }

        public override string Description
        {
            get
            {
                string description = resourceManager.GetString(resourceKey);
                return string.IsNullOrWhiteSpace(description) ? resourceKey : description;
            }
        }
    }
}
