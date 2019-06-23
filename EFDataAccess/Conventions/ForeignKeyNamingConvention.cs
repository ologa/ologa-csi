using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDataAccess.Conventions
{
    public class ForeignKeyNamingConvention : IStoreModelConvention<AssociationType>
    {
        public void Apply(AssociationType associationType, System.Data.Entity.Infrastructure.DbModel model)
        {
            if (associationType.IsForeignKey)
            {
                var constraint = associationType.Constraint;
                
                if (PropertiesHaveDefaultNames(constraint.FromProperties, constraint.ToRole.Name, constraint.ToProperties))
                {
                    ApplyCustomForeignKeyConvention(constraint.FromProperties);
                }

                if (PropertiesHaveDefaultNames(constraint.ToProperties, constraint.FromRole.Name, constraint.FromProperties))
                {
                    ApplyCustomForeignKeyConvention(constraint.ToProperties);
                }
            }
        }

        private void ApplyCustomForeignKeyConvention(ReadOnlyMetadataCollection<EdmProperty> properties)
        {
            for (int i = 0; i < properties.Count; i++)
            {
                int underscoreIndex = properties[i].Name.IndexOf('_');
                if (underscoreIndex > 0)
                {
                    properties[i].Name = properties[i].Name.Substring(0, underscoreIndex) + "ID";
                }
            }
        }

        private bool PropertiesHaveDefaultNames(ReadOnlyMetadataCollection<EdmProperty> fromProperties, string roleName, ReadOnlyMetadataCollection<EdmProperty> toProperties)
        {
            if (fromProperties.Count != toProperties.Count)
            {
                return false;
            }

            for (int i = 0; i < fromProperties.Count; i++)
            {
                if (!fromProperties[i].Name.EndsWith("_" + toProperties[i].Name))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
