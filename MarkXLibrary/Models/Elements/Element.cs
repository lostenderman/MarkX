using System.Collections.Generic;
using System.Linq;

namespace MarkX.Core
{
	public abstract class Element
	{
		public string? Id { get; set; }
		public string? MarkupName { get; set; }
		public List<Attr>? Attributes { get; set; }
		public List<NamePart>? NameParts { get; set; }
		public List<FallCondition>? FallConditions { get; set; }

		public bool IsEnabled { get; set; } = true;
		public virtual bool Separate { get; set; } = true;
		public bool ParenthesisesContent { get; set; } = true;
		public bool IncludeInBlocks { get; set; } = true;

		public string GetCurrentName(List<NamePart>? nameParts, Dictionary<string, string> currentAttributes, Dictionary<string, string>? currentVariables, bool isStart)
		{
			string name = "";
			if (nameParts == null)
			{
				return name;
			}
			foreach (var item in nameParts)
			{
				if ((isStart && !item.IncludeAtStart) || (!isStart && !item.IncludeAtEnd))
				{
					continue;
				}
				switch (item.Type)
				{
					case NamePartType.Attribute:
						if (Attributes == null || item.Value == null)
						{
							break;
						}
						var attribute = Attributes.Where(x => x.MarkupName == item.Value).FirstOrDefault();
						if (attribute == null || attribute.Pairs == null)
						{
							break;
						}
						var foundAttribute = currentAttributes.TryGetValue(item.Value, out var currentAttributeValue);
						if (!foundAttribute)
						{
							return "";
						}
						var pair = attribute.Pairs
							.Where(p => p.MarkupName == currentAttributeValue)
							.FirstOrDefault();
						name += pair?.MarkdownName ?? "";
						break;
					case NamePartType.Text:
						name += item.Value ?? "";
						break;
					case NamePartType.Variable:
						if (item.Value == "position")
						{
							name += isStart ? ResourceStrings.BlockPositionStart : ResourceStrings.BlockPositionEnd;
							break;
						}
						if (currentVariables == null || item.Value == null)
						{
							break;
						}
						var foundVariable = currentVariables.TryGetValue(item.Value, out var currentVariableValue);
						if (!foundVariable)
						{
							return "";
						}
						name += currentVariableValue;
						break;
					case NamePartType.Composite:
						var subname = GetCurrentName(item.SubNameParts, currentAttributes, currentVariables, true);
						name += subname;
						break;
					default:
						break;
				}
			}
			return name;
		}

	}
}