using System.Collections.Generic;
using System.Linq;

namespace MarkXLibrary
{
	public class MappingSpecification
	{
		public List<Element>? Elements { get; set; }
		public List<SpecialCharacter>? SpecialCharacters { get; set; }
		public List<InheritanceInfo>? Inheritances { get; set; }

		public Element? GetMappingDefinitionByName(string name, string value)
		{
			if (Elements == null)
			{
				return null;
			}
			foreach (var item in Elements)
			{
				if (item.MarkupName != name || !item.IsEnabled)
				{
					continue;
				}

				if ((item.FallConditions == null) || (!item.FallConditions.Any()))
				{
					return item;
				}

				if (item.FallConditions.Any(x => x.Condition != null && x.Condition(value)))
				{
					continue;
				}

				return item;
			}
			return null;
		}

		public Element? GetMappingDefinitionById(string id)
		{
			return Elements?.Where(x => x.Id == id).FirstOrDefault();
		}
	}
}