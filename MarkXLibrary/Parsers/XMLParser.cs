using System.Xml.Linq;

namespace MarkXLibrary
{
	public static class XMLParser
	{
		public static (IEnumerable<string>? lines, bool resetSeparation) Parse(XElement xElement, InheritanceData parentInheritance)
		{
			var result = new List<string>();
			var resetSeparation = false;
			var currentParentheses = parentInheritance.Parenthesise;

			var element = Mapping.DefaultMappingSpecification?.GetMappingDefinitionByName(xElement.Name.LocalName, xElement.Value);
			if (element == null || element.MarkupName == null || parentInheritance == null)
			{
				return (null, false);
			}

			var attributes = xElement.Attributes().ToDictionary(x => x.Name.LocalName, x => x.Value);
			var variables = new Dictionary<string, string>() { };
			var elementChildren = xElement.Elements();

			var childInheritance = new InheritanceData();

			#region Inheritance

			// update inheritance level
			foreach (var inheritance in parentInheritance.Inheritances)
			{
				inheritance.CurrentLevel++;
			}

			// pass inheritances
			var passableInheritances = parentInheritance.Inheritances.Where(x => x.IsPassable()).Select(x => (InheritanceInfo)x.Clone());
			if (passableInheritances != null)
			{
				childInheritance.Inheritances.AddRange(passableInheritances);
			}

			// create child inheritances
			var templateInheritances = Mapping.DefaultMappingSpecification?.Inheritances?.Where(x => x.ParentElement == element.MarkupName && x.Key != null && attributes.ContainsKey(x.Key));
			if (templateInheritances != null)
			{
				foreach (var inheritance in templateInheritances)
				{
					var inheritanceClone = (InheritanceInfo)inheritance.Clone();

					attributes.TryGetValue(inheritanceClone.Key ?? "", out var attributeValue);
					var mappedInheritanceValue = element.Attributes?
						.Where(x => x.MarkupName == inheritanceClone.Key)
						.FirstOrDefault()?.Pairs?
						.Where(x => x.MarkupName == attributeValue)
						.FirstOrDefault()?.MarkdownName;

					inheritanceClone.CurrentValue = mappedInheritanceValue ?? attributeValue;

					if (inheritanceClone.ValueInitOverride != null)
					{
						inheritanceClone.CurrentValue = inheritanceClone.ValueInitOverride;
					}
					childInheritance.Inheritances.Add(inheritanceClone);
				}
			}

			// use parent inheritances
			var applicableInheritances = parentInheritance.Inheritances?.Where(x => (x.ChildElements?.Contains(element.MarkupName) ?? false) && x.IsActivated());
			if (applicableInheritances != null)
			{
				foreach (var inheritance in applicableInheritances)
				{
					if (inheritance.Iteration > 0)
					{
						if (inheritance.Logic != null && inheritance.CurrentValue != null)
						{
							inheritance.CurrentValue = inheritance.Logic(inheritance.CurrentValue);
						}
					}
					inheritance.Iteration++;
					if (inheritance.Key != null && inheritance.CurrentValue != null)
					{
						variables.Add(inheritance.Key, inheritance.CurrentValue);
					}
				}
			}
			#endregion

			// get markers
			var startName = element.GetCurrentName(element.NameParts, attributes, variables, true);
			var endName = element.GetCurrentName(element.NameParts, attributes, variables, false);

			#region Block
			if (element is Block block)
			{
				if (block == null || block.MarkupName == null)
				{
					return (null, false);
				}

				// separate from previous element
				if (parentInheritance.Separate && block.Separate)
				{
					result.Add(ResourceStrings.BlockSeparator);
				}

				// add startname
				if ((block.NameParts?.Any() ?? false) && (block.IncludeInBlocks || parentInheritance.Parenthesise))
				{
					result.Add(startName);
				}

				if (xElement.IsEmpty)
				{
					goto All;
				}

				// iterate elements
				childInheritance.Separate = false;
				childInheritance.IncludeText = false;
				childInheritance.Parenthesise = false;
				ParseChildren(elementChildren, childInheritance, result);

				if ((block.NameParts?.Any() ?? false))
				{
					result.Add(endName);
				}
			}
			#endregion

			#region Multiline
			else if (element is Multiline multiline)
			{
				if (multiline.Attributes == null)
				{
					return (null, false);
				}

				// separate from previous element
				if (parentInheritance.Separate && multiline.Separate)
				{
					result.Add(ResourceStrings.BlockSeparator);
				}

				result.Add(ResourceStrings.MultilinePositionStart + startName);

				var mappedAttributes = multiline.Attributes.ToDictionary(x => x, x => attributes.Where(a => a.Key == x.MarkupName).FirstOrDefault().Value);
				foreach (var item in mappedAttributes)
				{
					var linePart = ResourceStrings.MultilineAttributeStart + item.Key.MarkdownName + ResourceStrings.InlineStart;

					if (item.Value != null)
					{
						var itemValue = item.Value;
						itemValue = string.Join("", itemValue.Escape(Mapping.DefaultMappingSpecification, item.Key.SpecialCharacterMapping, true, true, item.Key.IsInputPercentEncoded));

						linePart += itemValue;
					}
					else
					{
						var innerText = "";
						if (item.Key.IsValueHashed)
						{
							var valueToHash = xElement.Value;
							if (item.Key.StripsEndingNewlineInHash)
							{
								valueToHash = StringExtensions.StripLastNewline(valueToHash);
							}
							innerText = StringExtensions.HashToVerbatim(valueToHash);
						}
						else
						{
							var sub = new List<string>();
							childInheritance.Separate = false;
							childInheritance.IncludeText = true;
							childInheritance.Parenthesise = true;
							ParseChildren(elementChildren, childInheritance, sub);
							innerText = string.Join("", sub);
						}

						linePart += innerText;
					}
					result.Add(linePart);
				}

				result.Add(ResourceStrings.MultilinePositionEnd + endName);
			}
			#endregion

			#region Inline
			else if (element is Inline inline)
			{
				// separate from previous element
				if (parentInheritance.Separate && inline.Separate)
				{
					result.Add(ResourceStrings.BlockSeparator);
				}

				if (!inline.IncludeContent)
				{
					result.Add(startName);
					goto All;
				}

				var sub = new List<string>();
				if (inline.IsValueHashed)
				{
					var valueToHash = xElement.Value;
					if (inline.StripEndingNewlineInHash)
					{
						valueToHash = StringExtensions.StripLastNewline(valueToHash);
					}
					sub.Add(StringExtensions.HashToVerbatim(valueToHash));
				}
				else
				{
					var inlineDecoded = xElement.Value.Escape(Mapping.DefaultMappingSpecification, inline.SpecialCharacterMapping, true, true, false);
					var inlineDecodedJoined = string.Join("", inlineDecoded);
					if (!elementChildren.Any())
					{
						if (!string.IsNullOrWhiteSpace(inlineDecodedJoined) || inline.AllowsEmpty)
						{
							if (inlineDecodedJoined.Any())
							{
								sub.Add(StringExtensions.ReplaceNewlines(inlineDecodedJoined));
							}
							else
							{
								sub.Add(StringExtensions.ReplaceNewlines(xElement.Value));
							}
						}
					}

					childInheritance.Separate = false;
					childInheritance.IncludeText = true;
					childInheritance.Parenthesise = true;
					ParseChildren(elementChildren, childInheritance, sub);
				}

				var inlineResult = string.Join("", sub);
				if (inline.Format != null)
				{
					inlineResult = inline.Format(inlineResult);
				}
				result.Add(startName + ResourceStrings.InlineStart + inlineResult);
			}
			#endregion

			#region Atom
			else if (element is Atom atom)
			{
				resetSeparation = true;
				var decoded = xElement.Value.Escape(Mapping.DefaultMappingSpecification, atom.SpecialCharacterMapping, parentInheritance.IncludeText, parentInheritance.Parenthesise, false);
				var joinChar = parentInheritance.Parenthesise ? "" : "\n";
				var decodedJoined = string.Join(joinChar, decoded);
				if (decodedJoined.Any())
				{
					if (parentInheritance.Parenthesise)
					{
						result.Add(StringExtensions.ReplaceNewlines(decodedJoined));
					}
					else
					{
						result.Add(decodedJoined);
					}
				}
				currentParentheses = false;
			}
			#endregion

			else
			{
				// TODO
				throw new Exception("Object not an Element");
			}

		All:

			if (currentParentheses && element.ParenthesisesContent)
			{
				result = StringExtensions.AddParentheses(result).ToList();
			}
			return (result, resetSeparation);
		}

		private static void ParseChildren(IEnumerable<XElement> elementChildren, InheritanceData childInheritance, List<string> sub)
		{
			foreach (var child in elementChildren)
			{
				var parsedChild = Parse(child, childInheritance);
				if (parsedChild.lines != null)
				{
					sub.AddRange(parsedChild.lines);
				}
				childInheritance.Separate = true;
				if (parsedChild.resetSeparation)
				{
					childInheritance.Separate = false;
				}
			}
		}
	}
}