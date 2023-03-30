namespace MarkX.Core
{
	// TODO make non-static -> current mapping at least
	public static class Mapping
	{
		public static MappingSpecification? DefaultMappingSpecification { get; set; } = new MappingSpecification()
		{
			Elements = new List<Element>()
			{
				#region Blocks
				new Block()
				{
					Id = "list",
					MarkupName = "list",
					Attributes = new List<Attr>()
					{
						new Attr()
						{
							MarkdownName = "",
							MarkupName = "type",
							Pairs = new List<Pair>()
							{
								new Pair()
								{
									MarkdownName = "ol",
									MarkupName = "ordered",
								},
								new Pair()
								{
									MarkdownName = "ul",
									MarkupName = "bullet",
								}
							}
						},
						new Attr()
						{
							MarkdownName = "",
							MarkupName = "tight",
							Pairs = new List<Pair>()
							{
								new Pair()
								{
									MarkdownName = "Tight",
									MarkupName = "true",
								},
								new Pair()
								{
									MarkdownName = "",
									MarkupName = "false",
								}
							}
						},
						new Attr()
						{
							MarkdownName = "",
							MarkupName = "start",
						},
						new Attr()
						{
							MarkdownName = "",
							MarkupName = "delimeter",
						}
					},
					NameParts = new List<NamePart>()
					{
						new NamePart()
						{
							Type = NamePartType.Attribute,
							Value = "type"
						},
						new NamePart()
						{
							Type = NamePartType.Variable,
							Value = "position"
						},
						new NamePart()
						{
							Type = NamePartType.Attribute,
							Value = "tight"
						}
					}
				},
				new Block()
				{
					Id = "document",
					MarkupName = "document",
					NameParts = new List<NamePart>()
					{
						new NamePart()
						{
							Type = NamePartType.Text,
							Value = "document"
						},
						new NamePart()
						{
							Type = NamePartType.Variable,
							Value = "position"
						},
					}
				},
				new Block()
				{
					Id = "paragraph",
					MarkupName = "paragraph",
				},
				new Block()
				{
					Id = "linebreak",
					MarkupName = "linebreak",
					ParenthesisesContent = false,
					IncludeInBlocks = false,
					Separate = false,
					NameParts = new List<NamePart>()
					{
						new NamePart()
						{
							Type = NamePartType.Text,
							Value = " "
						},
					},
				},
				new Block()
				{
					Id = "softbreak",
					MarkupName = "softbreak",
					ParenthesisesContent = false,
					IncludeInBlocks = false,
					Separate = false,
					NameParts = new List<NamePart>()
					{
						new NamePart()
						{
							Type = NamePartType.Text,
							Value = " "
						},
					},
				},
				new Block()
				{
					Id = "thematic_break",
					MarkupName = "thematic_break",
					NameParts = new List<NamePart>()
					{
						new NamePart()
						{
							Type = NamePartType.Text,
							Value = "thematicBreak"
						},
					},
				},
				new Block()
				{
					Id = "item",
					MarkupName = "item",
					Separate = false,
					NameParts = new List<NamePart>()
					{
						new NamePart()
						{
							Type = NamePartType.Variable,
							Value = "type"
						},
						new NamePart()
						{
							Type = NamePartType.Text,
							Value = "Item"
						},
						new NamePart()
						{
							Type = NamePartType.Composite,
							IncludeAtEnd = false,
							SubNameParts = new List<NamePart>()
							{
								new NamePart()
								{
									Type = NamePartType.Text,
									Value = "WithNumber: ",
								},
								new NamePart()
								{
									Type = NamePartType.Variable,
									Value = "start",
								}
							}
						},
						new NamePart()
						{
							Type = NamePartType.Variable,
							Value = "position",
							IncludeAtStart = false,
						},
					},
				},
				new Block()
				{
					Id = "block_quote",
					MarkupName = "block_quote",
					NameParts = new List<NamePart>()
					{
						new NamePart()
						{
							Type = NamePartType.Text,
							Value = "blockQuote"
						},
						new NamePart()
						{
							Type = NamePartType.Variable,
							Value = "position"
						},
					},
				},
				new Block()
				{
					Id = "html_block",
					MarkupName = "html_block",
					NameParts = new List<NamePart>()
					{
						new NamePart()
						{
							Type = NamePartType.Text,
							Value = "blockHtmlComment",
						},
						new NamePart()
						{
							Type = NamePartType.Variable,
							Value = "position",
						},
					},
					FallConditions = new List<FallCondition>()
					{
						new FallCondition()
						{
							Key = "IsNotHtmlComment",
							Type = FallConditionType.Value,
							Condition = x =>
							{
								var indexStart = x.IndexOf(ResourceStrings.HtmlCommentStart);
								var indexEnd = x.IndexOf(ResourceStrings.HtmlCommentEnd);
								if (indexStart != 0 || indexEnd == -1)
								{
									return true;
								}
								var inner = x.Substring(indexStart + 4, indexEnd - indexStart - 4);
								return
									inner.StartsWith(">") ||
									inner.StartsWith("->") ||
									inner.EndsWith("-") ||
									inner.Contains("--");
							}
						}
					}
				},
				#endregion

				#region Multilines
				new Multiline()
				{
					Id = "link",
					MarkupName = "link",
					Attributes = new List<Attr>()
					{
						new Attr()
						{
							MarkdownName = "label", // inner text
							MarkupName = "",
						},
						new Attr()
						{
							MarkdownName = "URI",
							MarkupName = "destination",
							SpecialCharacterMapping = SpecialCharacterType.Uri,
							IsInputPercentEncoded = true
						},
						new Attr()
						{
							MarkdownName = "title",
							MarkupName = "title",
						}
					},
					NameParts = new List<NamePart>()
					{
						new NamePart()
						{
							Type = NamePartType.Text,
							Value = "link"
						},
					}
				},
				new Multiline()
				{
					Id = "image",
					MarkupName = "image",
					Attributes = new List<Attr>()
					{
						new Attr()
						{
							MarkdownName = "label", // inner text
							MarkupName = "",
						},
						new Attr()
						{
							MarkdownName = "URI",
							MarkupName = "destination",
							SpecialCharacterMapping = SpecialCharacterType.Uri,
							IsInputPercentEncoded = true
						},
						new Attr()
						{
							MarkdownName = "title",
							MarkupName = "title",
						}
					},
					NameParts = new List<NamePart>()
					{
						new NamePart()
						{
							Type = NamePartType.Text,
							Value = "image"
						},
					}
				},
				new Multiline()
				{
					Id = "fenced_code_block",
					MarkupName = "code_block",
					Separate = true,
					Attributes = new List<Attr>()
					{
						new Attr()
						{
							MarkdownName = "src",
							MarkupName = "",
							IsValueHashed = true,
							StripsEndingNewlineInHash = true,
						},
						new Attr()
						{
							MarkdownName = "infostring",
							MarkupName = "info",
							SpecialCharacterMapping = SpecialCharacterType.Uri
						}
					},
					NameParts = new List<NamePart>()
					{
						new NamePart()
						{
							Type = NamePartType.Text,
							Value = "fencedCode"
						},
					}
				},
				#endregion

				#region Inlines
				new Inline()
				{
					Id = "strong",
					MarkupName = "strong",
					NameParts = new List<NamePart>()
					{
						new NamePart()
						{
							Type = NamePartType.Text,
							Value = "strongEmphasis"
						},
					}
				},
				new Inline()
				{
					Id = "emph",
					MarkupName = "emph",
					NameParts = new List<NamePart>()
					{
						new NamePart()
						{
							Type = NamePartType.Text,
							Value = "emphasis"
						},
					}
				},
				new Inline()
				{
					Id = "code",
					MarkupName = "code",
					NameParts = new List<NamePart>()
					{
						new NamePart()
						{
							Type = NamePartType.Text,
							Value = "codeSpan"
						},
					}
				},
				new Inline()
				{
					Id = "heading",
					MarkupName = "heading",
					Separate = true,
					AllowsEmpty = false,
					Attributes = new List<Attr>()
					{
						new Attr()
						{
							MarkdownName = "",
							MarkupName = "level",
							Pairs = new List<Pair>()
							{
								new Pair()
								{
									MarkdownName = "One",
									MarkupName = "1",
								},
								new Pair()
								{
									MarkdownName = "Two",
									MarkupName = "2",
								},
								new Pair()
								{
									MarkdownName = "Three",
									MarkupName = "3",
								},
								new Pair()
								{
									MarkdownName = "Four",
									MarkupName = "4",
								},
								new Pair()
								{
									MarkdownName = "Five",
									MarkupName = "5",
								},
								new Pair()
								{
									MarkdownName = "Six",
									MarkupName = "6",
								}
							}
						},
					},
					NameParts = new List<NamePart>()
					{
						new NamePart()
						{
							Type = NamePartType.Text,
							Value = "heading"
						},
						new NamePart()
						{
							Type = NamePartType.Attribute,
							Value = "level"
						}
					}
				},
				new Inline()
				{
					Id = "html_block",
					MarkupName = "html_block",
					Separate = true,
					IsValueHashed = true,
					NameParts = new List<NamePart>()
					{
						new NamePart()
						{
							Type = NamePartType.Text,
							Value = "inputBlockHtmlElement",
						},
					},
				},
				new Inline()
				{
					Id = "html_inline",
					MarkupName = "html_inline",
					Format = x => {
						var commentStart = x.IndexOf(ResourceStrings.HtmlCommentStart) + ResourceStrings.HtmlCommentStart.Length;
						var commentEnd = x.IndexOf(ResourceStrings.HtmlCommentEnd);
						var inner = x[commentStart..commentEnd ];
						return inner;
					},
					NameParts = new List<NamePart>()
					{
						new NamePart()
						{
							Type = NamePartType.Text,
							Value = "inlineHtmlComment",
						},
					},
					FallConditions = new List<FallCondition>()
					{
						new FallCondition()
						{
							Key = "IsNotHtmlComment",
							Type = FallConditionType.Value,
							Condition = x =>
							{
								var indexStart = x.IndexOf(ResourceStrings.HtmlCommentStart);
								var indexEnd = x.IndexOf(ResourceStrings.HtmlCommentEnd);
								if (indexStart != 0 || indexEnd == -1)
								{
									return true;
								}
								var commentStart = x.IndexOf(ResourceStrings.HtmlCommentStart) + ResourceStrings.HtmlCommentStart.Length;
								var commentEnd = x.IndexOf(ResourceStrings.HtmlCommentEnd);
								var inner = x[commentStart..commentEnd ];
								return
									inner.StartsWith(">") ||
									inner.StartsWith("->") ||
									inner.EndsWith("-") ||
									inner.Contains("--");
							}
						}
					}
				},
				new Inline()
				{
					Id = "html_inline",
					MarkupName = "html_inline",
					NameParts = new List<NamePart>()
					{
						new NamePart()
						{
							Type = NamePartType.Text,
							Value = "inlineHtmlTag",
						},
					},
				},
				new Inline()
				{
					Id = "indented_code_block",
					MarkupName = "code_block",
					IsValueHashed = true,
					Separate = true,
					StripEndingNewlineInHash = true,
					NameParts = new List<NamePart>()
					{
						new NamePart()
						{
							Type = NamePartType.Text,
							Value = "inputVerbatim",
						},
					},
				},
				#endregion

				#region Atoms
				new Atom()
				{
					Id = "text",
					MarkupName = "text",
				},
				#endregion
			},
			SpecialCharacters = new List<SpecialCharacter>()
			{
				new SpecialCharacter()
				{
					MarkdownName = "nbsp",
					MarkupName = ' ',
					SpecialCharacterTypes = new List<SpecialCharacterType>()
					{
						SpecialCharacterType.Basic
					}
				},
				new SpecialCharacter()
				{
					MarkdownName = "hash",
					MarkupName = '#',
					SpecialCharacterTypes = new List<SpecialCharacterType>()
					{
						SpecialCharacterType.Basic
					}
				},
				new SpecialCharacter()
				{
					MarkdownName = "dollarSign",
					MarkupName = '$',
					SpecialCharacterTypes = new List<SpecialCharacterType>()
					{
						SpecialCharacterType.Basic
					}
				},
				new SpecialCharacter()
				{
					MarkdownName = "percentSign",
					MarkupName = '%',
					SpecialCharacterTypes = new List<SpecialCharacterType>()
					{
						SpecialCharacterType.Basic
					}
				},
				new SpecialCharacter()
				{
					MarkdownName = "ampersand",
					MarkupName = '&',
					SpecialCharacterTypes = new List<SpecialCharacterType>()
					{
						SpecialCharacterType.Basic
					}
				},
				new SpecialCharacter()
				{
					MarkdownName = "backslash",
					MarkupName = '\\',
					SpecialCharacterTypes = new List<SpecialCharacterType>()
					{
						SpecialCharacterType.Basic,
						SpecialCharacterType.Uri
					}
				},
				new SpecialCharacter()
				{
					MarkdownName = "circumflex",
					MarkupName = '^',
					SpecialCharacterTypes = new List<SpecialCharacterType>()
					{
						SpecialCharacterType.Basic
					}
				},
				new SpecialCharacter()
				{
					MarkdownName = "underscore",
					MarkupName = '_',
					SpecialCharacterTypes = new List<SpecialCharacterType>()
					{
						SpecialCharacterType.Basic
					}
				},
				new SpecialCharacter()
				{
					MarkdownName = "leftBrace",
					MarkupName = '{',
					SpecialCharacterTypes = new List<SpecialCharacterType>()
					{
						SpecialCharacterType.Basic,
						SpecialCharacterType.Uri
					}
				},
				new SpecialCharacter()
				{
					MarkdownName = "rightBrace",
					MarkupName = '}',
					SpecialCharacterTypes = new List<SpecialCharacterType>()
					{
						SpecialCharacterType.Basic,
						SpecialCharacterType.Uri
					}
				},
				new SpecialCharacter()
				{
					MarkdownName = "pipe",
					MarkupName = '|',
					SpecialCharacterTypes = new List<SpecialCharacterType>()
					{
						SpecialCharacterType.Basic
					}
				},
				new SpecialCharacter()
				{
					MarkdownName = "tilde",
					MarkupName = '~',
					SpecialCharacterTypes = new List<SpecialCharacterType>()
					{
						SpecialCharacterType.Basic
					}
				}
			},
			Inheritances = new List<InheritanceInfo>()
			{
				new InheritanceInfo()
				{
					Key = "start",
					ParentElement = "list",
					ChildElements = new List<string>()
					{
						"item"
					},
					Logic = x => int.TryParse(x, out var xInt) ? (xInt + 1).ToString() : "",
				},
				new InheritanceInfo()
				{
					Key = "type",
					ParentElement = "list",
					ChildElements = new List<string>()
					{
						"item"
					},
					Logic = x => x
				}
			},
		};

		public static List<Extension>? Extensions { get; set; } = new List<Extension>()
		{
			new Extension
			{
				Key = "line_blocks",
				Specification = new MappingSpecification()
				{
					Elements = new List<Element>(){
						new Block()
						{
							MarkupName = "line_block",
							NameParts = new List<NamePart>()
							{
								new NamePart()
								{
									Type = NamePartType.Text,
									Value = "lineBlock"
								},
								new NamePart()
								{
									Type = NamePartType.Variable,
									Value = "position"
								},
							},
						},
					}
				}
			},
			new Extension
			{
				Key = "strike_through",
				Specification = new MappingSpecification()
				{
					Elements = new List<Element>()
					{
						new Inline()
						{
							MarkupName = "strike_through",
							NameParts = new List<NamePart>()
							{
								new NamePart()
								{
									Type = NamePartType.Text,
									Value = "strikeThrough"
								},
							}
						},
					}
				}
			},
			new Extension
			{
				Key = "tex_math_dollars",
				Specification = new MappingSpecification()
				{
					Elements = new List<Element>(){
						new Inline()
						{
							MarkupName = "display_math",
							IncludeContent = false,
							NameParts = new List<NamePart>()
							{
								new NamePart()
								{
									Type = NamePartType.Text,
									Value = "displayMath"
								},
							},
						},
						new Inline()
						{
							MarkupName = "inline_math",
							IncludeContent = false,
							NameParts = new List<NamePart>()
							{
								new NamePart()
								{
									Type = NamePartType.Text,
									Value = "inlineMath"
								},
							},
						},
					}
				}
			},
		};

		public static void ApplyExtensions(IEnumerable<string>? extensions)
		{
			if (extensions == null)
			{
				return;
			}
			foreach (var possibleExtensionName in extensions)
			{
				var extension = Extensions?.Where(x => x.Key == possibleExtensionName).FirstOrDefault();
				if (extension == null)
				{
					return;
				}
				if (extension.Specification?.Elements != null)
				{
					DefaultMappingSpecification?.Elements?.AddRange(extension.Specification.Elements);
				}
				if (extension.Specification?.SpecialCharacters != null)
				{
					DefaultMappingSpecification?.SpecialCharacters?.AddRange(extension.Specification.SpecialCharacters);
				}
				if (extension.Specification?.Inheritances != null)
				{
					DefaultMappingSpecification?.Inheritances?.AddRange(extension.Specification.Inheritances);
				}
			}
			return;
		}
	}
}