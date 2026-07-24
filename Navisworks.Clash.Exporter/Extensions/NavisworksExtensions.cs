using System;
using Autodesk.Navisworks.Api;

namespace Navisworks.Clash.Exporter.Extensions
{
    public static class NavisworksExtensions
    {
        /// <summary>Converts a Navisworks internal length (feet) to millimetres.</summary>
        public static double ToMillimetres(this double value)
        {
            return value * UnitConversion.ScaleFactor(Units.Feet, Units.Millimeters);
        }

        /// <summary>
        /// Walks up the model tree until it finds an item that carries a stable identifier
        /// (Revit instance GUID, AutoCAD handle, GUID tab or Microstation element id).
        /// </summary>
        public static ModelItem GetUniquelyIdentifiableItem(this ModelItem item, out string uniqueId)
        {
            string id;
            var currentItem = item;

            while (true)
            {
                var guid = currentItem.InstanceGuid; // Revit GUID
                if (guid == Guid.Empty)
                    id = currentItem.FromAutoCAD() ?? currentItem.FromGuidTab() ?? currentItem.FromMicrostation();
                else
                    id = guid.ToString();

                if (id != null) break;
                if (currentItem.Parent == null)
                {
                    currentItem = null;
                    break;
                }

                currentItem = currentItem.Parent;
            }

            uniqueId = id;
            return currentItem;
        }

        private static string FromAutoCAD(this ModelItem item)
        {
            var cat = item.PropertyCategories.FindCategoryByName("LcOpDwgEntityAttrib") ??
                      item.PropertyCategories.FindCategoryByDisplayName("Entity Handle");
            return ReadValue(cat);
        }

        private static string FromGuidTab(this ModelItem item)
        {
            var cat = item.PropertyCategories.FindCategoryByName("LcArGUID") ??
                      item.PropertyCategories.FindCategoryByDisplayName("GUID");
            return ReadValue(cat);
        }

        private static string FromMicrostation(this ModelItem item)
        {
            var cat = item.PropertyCategories.FindCategoryByDisplayName("Element ID");
            return ReadValue(cat);
        }

        private static string ReadValue(PropertyCategory cat)
        {
            if (cat == null) return null;
            var value = cat.Properties.FindPropertyByName("LcOaNat64AttributeValue") ??
                        cat.Properties.FindPropertyByDisplayName("Value");
            return value?.Value.ToDisplayString();
        }
    }
}
