<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="3.0"
xmlns:cm="http://commonmark.org/xml/1.0" xmlns:ext="mark:ext">
    <xsl:output method="text" omit-xml-declaration="yes" encoding="UTF-8" indent="no"/>

	<xsl:param name="indented-code"></xsl:param>
	<xsl:param name="extensions"></xsl:param>
	
	<xsl:variable name="block-separator" select="'interblockSeparator'"/>
	<xsl:variable name="block-position-start" select="'Begin'"/>
	<xsl:variable name="block-position-end" select="'End'"/>
	<xsl:variable name="multiline-position-start" select="'BEGIN '"/>
	<xsl:variable name="multiline-position-end" select="'END '"/>
	<xsl:variable name="multiline-attribute-start" select="'- '"/>
	<xsl:variable name="inline-start" select="': '"/>
	<xsl:variable name="verbatim-directory" select="'./_markdown_test/'"/>
	<xsl:variable name="verbatim" select="'.verbatim'"/>

	<!-- HELPERS -->

	<xsl:template name="remove-last">
		<xsl:param name="str"/>

		<xsl:choose>
			<xsl:when test="substring($str, string-length($str), 1) = '&#10;'">
				<xsl:value-of select="substring($str, 1, string-length($str) - 1)" />
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$str" />
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template name="is-extension-enabled">
		<xsl:param name="current-extensions"/>
		<xsl:param name="extension-name"/>
				
		<xsl:if test="$current-extensions != ''">
			
			<xsl:variable name="first-extension" select="substring-before($current-extensions, ' ')"/>
			<xsl:variable name="remaining-extensions" select="substring-after($current-extensions, ' ')"/>
			
			<xsl:choose>
				<xsl:when test="$first-extension = ''">
					<xsl:if test="$current-extensions = $extension-name">
						<xsl:value-of select="'true()'" />
					</xsl:if>
				</xsl:when>
				<xsl:otherwise>
					<xsl:choose>
						<xsl:when test="$first-extension = $extension-name">
							<xsl:value-of select="'true()'" />
						</xsl:when>
						<xsl:otherwise>
							<xsl:call-template name="is-extension-enabled">
								<xsl:with-param name="extension-name" select="$extension-name" />
								<xsl:with-param name="current-extensions" select="$remaining-extensions"/>
							</xsl:call-template>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:if>
	</xsl:template>

	<xsl:template name="replace-newlines-with-spaces">
		<xsl:param name="text"/>

		<xsl:value-of select="translate($text, '&#10;', ' ')"/>
	</xsl:template>
	
	<xsl:template name="enclose-hash">
		<xsl:param name="hashed-content"/>
		
		<xsl:value-of select="$verbatim-directory"/>
		<xsl:value-of select="$hashed-content"/>
		<xsl:value-of select="$verbatim"/>
	</xsl:template>
	
	<xsl:template name="multiline-attribute">
		<xsl:param name="name"/>
		<xsl:param name="value"/>
		
		<xsl:value-of select="$multiline-attribute-start"/>
		<xsl:value-of select="$name"/>
		<xsl:value-of select="$inline-start"/>
		<xsl:value-of select="$value"/>
		<xsl:text>&#10;</xsl:text>
	</xsl:template>
	
	<xsl:template name="is-inline-html-comment">
		<xsl:param name="content"/>
		<xsl:value-of select="
		starts-with($content, '&lt;!--') and
		substring($content, string-length($content) - string-length('--&gt;') + 1) = '--&gt;' and
		string-length($content) &gt;= 7
		"/>
	</xsl:template>

	<xsl:template name="extract-comment-content">
		<xsl:param name="content" />
		<xsl:value-of select="substring($content, 5, string-length($content) - 7)"/>
	</xsl:template>

	<xsl:template name="is-valid-comment-content">
		<xsl:param name="comment-content"/>
		<xsl:value-of select="
		not(starts-with($comment-content, '&gt;')) and
		not(starts-with($comment-content, '-&gt;')) and
		not(substring($comment-content, string-length($comment-content) - string-length('-') + 1) = '-') and
		not(contains($comment-content, '--'))
		"/>
	</xsl:template>

	<!-- ROOT -->

	<xsl:template match="/">
        <xsl:apply-templates select="cm:document"/>
    </xsl:template>

	<!-- BLOCKS -->
	
	<xsl:template name="general-block">
		<xsl:param name="begin-name"/>
		<xsl:param name="end-name"/>

		<xsl:value-of select="$begin-name"/>
		<xsl:text>&#10;</xsl:text>
		<xsl:call-template name="blocks"/>
		<xsl:value-of select="$end-name"/>
		<xsl:text>&#10;</xsl:text>
	</xsl:template>
		
	<xsl:template name="blocks">
		<xsl:for-each select="cm:paragraph|cm:list|cm:html_block|cm:block_quote|cm:thematic_break|cm:code_block|cm:heading">
			<xsl:apply-templates select="."></xsl:apply-templates>
			<xsl:if test="position() != last()">
				<xsl:copy-of select="$block-separator"/>
				<xsl:text>&#10;</xsl:text>
			</xsl:if>
		</xsl:for-each>
		<xsl:for-each select="cm:linebreak|cm:item">
			<xsl:apply-templates select="."></xsl:apply-templates>
		</xsl:for-each>
	</xsl:template>

    <xsl:template match="cm:document">
        <xsl:text>documentBegin</xsl:text>
		<xsl:text>&#10;</xsl:text>
		<xsl:call-template name="blocks"/>
        <xsl:text>documentEnd</xsl:text>
		<xsl:text>&#10;</xsl:text>
    </xsl:template>

    <xsl:template match="cm:paragraph">
        <xsl:apply-templates select="cm:*"/>
    </xsl:template>

	<xsl:template match="cm:softbreak">
	</xsl:template>

	<xsl:template name="inline-softbreak">
		<xsl:text> </xsl:text>
	</xsl:template>

	<xsl:template match="cm:linebreak">
		<xsl:text>hardLineBreak</xsl:text>
		<xsl:text>&#10;</xsl:text>
	</xsl:template>

	<xsl:template match="cm:thematic_break">
		<xsl:text>thematicBreak</xsl:text>
		<xsl:text>&#10;</xsl:text>
	</xsl:template>

	<xsl:template name="list-content">
		<xsl:param name="list-type"/>

		<xsl:variable name="tight" select="'Tight'"/>

		<xsl:copy-of select="$list-type"/>
		<xsl:copy-of select="$block-position-start"/>
		<xsl:if test="@tight = 'true'">
			<xsl:copy-of select="$tight"/>
		</xsl:if>
		<xsl:text>&#10;</xsl:text>
		
		<!-- ONLY ITEMS CAN BE HERE (ONE TYPE) -->
		<xsl:call-template name="blocks"/>

		<xsl:copy-of select="$list-type"/>
		<xsl:copy-of select="$block-position-end"/>
		<xsl:if test="@tight = 'true'">
			<xsl:copy-of select="$tight"/>
		</xsl:if>
	</xsl:template>
	
	<xsl:template match="cm:block_quote">
        <xsl:text>blockQuoteBegin</xsl:text>
		<xsl:text>&#10;</xsl:text>
        <xsl:call-template name="blocks"/>
        <xsl:text>blockQuoteEnd</xsl:text>
		<xsl:text>&#10;</xsl:text>
    </xsl:template>

	<xsl:template match="cm:list">	
		<xsl:choose>
			<xsl:when test="@type = 'bullet'">
				<xsl:call-template name="list-content">
					<xsl:with-param name="list-type" select="'ul'" />
				</xsl:call-template>
			</xsl:when>
			<xsl:when test="@type = 'ordered'">
				<xsl:call-template name="list-content">
					<xsl:with-param name="list-type" select="'ol'" />
				</xsl:call-template>
			</xsl:when>
		</xsl:choose>
		<xsl:text>&#10;</xsl:text>
	</xsl:template>
	
	<!-- TODO SIMPLIFY -->
	<xsl:template match="cm:item">
		<xsl:choose>
			<xsl:when test="../@type = 'bullet'">
				<xsl:text>ul</xsl:text>
				<xsl:text>Item</xsl:text>
				<xsl:text>&#10;</xsl:text>
				<xsl:call-template name="blocks"/>
				<xsl:text>ul</xsl:text>
				<xsl:text>Item</xsl:text>
				<xsl:text>End</xsl:text>
			</xsl:when>
			<xsl:when test="../@type = 'ordered'">
				<xsl:text>ol</xsl:text><xsl:text>Item</xsl:text><xsl:text>WithNumber</xsl:text>: <xsl:value-of select="../@start + position() - 1"/>
				<xsl:text>&#10;</xsl:text>
				<xsl:call-template name="blocks"/>
				<xsl:text>ol</xsl:text><xsl:text>Item</xsl:text><xsl:text>End</xsl:text>
			</xsl:when>
		</xsl:choose>
		<xsl:text>&#10;</xsl:text>
	</xsl:template>

	<!-- INLINE -->

	<xsl:template name="inline">
		<xsl:variable name="sub">
			<xsl:for-each select="cm:*">
				<xsl:choose>
					<xsl:when test="local-name() = 'text'">
						<xsl:apply-templates select="." mode="inline"/>
					</xsl:when>
					<xsl:when test="local-name() = 'softbreak'">
						<xsl:call-template name="inline-softbreak"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:variable name="els">
							<xsl:apply-templates select="." />
						</xsl:variable>
					
						<xsl:call-template name="parenthesise-group">
							<xsl:with-param name="str" select="$els"/>
						</xsl:call-template>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:for-each>
		</xsl:variable>

		<xsl:value-of select="$sub"/>
	</xsl:template>

	<xsl:template name="general-inline">
		<xsl:param name="inline-name"/>
		<xsl:param name="inline-content"/>

		<xsl:value-of select="$inline-name"/>
		<xsl:copy-of select="$inline-start"/>
		<xsl:value-of select="$inline-content"/>
		<xsl:text>&#10;</xsl:text>
	</xsl:template>
    
	<xsl:template match="cm:heading">
		<xsl:variable name="heading-name">
			<xsl:text>heading</xsl:text>
			<xsl:choose>
				<xsl:when test="@level = 1">One</xsl:when>
				<xsl:when test="@level = 2">Two</xsl:when>
				<xsl:when test="@level = 3">Three</xsl:when>
				<xsl:when test="@level = 4">Four</xsl:when>
				<xsl:when test="@level = 5">Five</xsl:when>
        		<xsl:when test="@level = 6">Six</xsl:when>
			</xsl:choose>
		</xsl:variable>
	
		<xsl:call-template name="general-inline">
			<xsl:with-param name="inline-name" select="$heading-name"/>
			<xsl:with-param name="inline-content">
				<xsl:call-template name="inline"/>
			</xsl:with-param>
		</xsl:call-template>
    </xsl:template>

	<xsl:template match="cm:emph">
		<xsl:call-template name="general-inline">
			<xsl:with-param name="inline-name" select="'emphasis'"/>
			<xsl:with-param name="inline-content">
				<xsl:call-template name="inline"/>
			</xsl:with-param>
		</xsl:call-template>
    </xsl:template>

	<xsl:template match="cm:strong">
		<xsl:call-template name="general-inline">
			<xsl:with-param name="inline-name" select="'strongEmphasis'"/>
			<xsl:with-param name="inline-content">
				<xsl:call-template name="inline"/>
			</xsl:with-param>
		</xsl:call-template>
	</xsl:template>
	
	<xsl:template match="cm:code">
		<xsl:call-template name="general-inline">
			<xsl:with-param name="inline-name" select="'codeSpan'"/>
			<xsl:with-param name="inline-content">
				<xsl:call-template name="escape-text">
					<xsl:with-param name="text" select="."/>
					<xsl:with-param name="inline" select="'true'"/>
					<xsl:with-param name="map-type" select="'typographic'"/>
				</xsl:call-template>
			</xsl:with-param>
		</xsl:call-template>
	</xsl:template>
	
	<xsl:template match="cm:text">
		<xsl:variable name="content" select="."/>

		<xsl:variable name="content-with-replaced-newlines">
			<xsl:call-template name="replace-newlines-with-spaces">
				<xsl:with-param name="text" select="$content"/>
			</xsl:call-template>
		</xsl:variable>
		
		<xsl:call-template name="escape-text">
			<xsl:with-param name="text" select="$content-with-replaced-newlines"/>
			<xsl:with-param name="map-type" select="'typographic'"/>
		</xsl:call-template>
		
	</xsl:template>

	<xsl:template match="cm:text" mode="inline">
		<xsl:variable name="content" select="."/>

		<xsl:variable name="content-with-replaced-newlines">
			<xsl:call-template name="replace-newlines-with-spaces">
				<xsl:with-param name="text" select="$content"/>
			</xsl:call-template>
		</xsl:variable>

		<xsl:call-template name="escape-text">
			<xsl:with-param name="text" select="$content-with-replaced-newlines"/>
			<xsl:with-param name="inline" select="'true'"/>
			<xsl:with-param name="map-type" select="'typographic'"/>
		</xsl:call-template>
	
	</xsl:template>

	<xsl:template match="cm:link | cm:image">
		<xsl:copy-of select="$multiline-position-start"/>
		<xsl:value-of select ="name(.)"/>
		<xsl:text>&#10;</xsl:text>
		
		<xsl:call-template name="multiline-attribute">
			<xsl:with-param name="name" select="'label'"/>
			<xsl:with-param name="value">
				<xsl:call-template name="inline"/>
			</xsl:with-param>
		</xsl:call-template>
		
		<xsl:call-template name="multiline-attribute">
			<xsl:with-param name="name" select="'URI'"/>
			<xsl:with-param name="value">
				<xsl:call-template name="escape-text">
					<xsl:with-param name="text" select="ext:UnescapeUri(@destination)"/>
					<xsl:with-param name="inline" select="'true'"/>
					<xsl:with-param name="map-type" select="'programmatic'"/>
				</xsl:call-template>
			</xsl:with-param>
		</xsl:call-template>
		
		<xsl:call-template name="multiline-attribute">
			<xsl:with-param name="name" select="'title'"/>
			<xsl:with-param name="value">
				<xsl:call-template name="escape-text">
					<xsl:with-param name="text" select="@title"/>
					<xsl:with-param name="inline" select="'true'"/>
					<xsl:with-param name="map-type" select="'typographic'"/>
				</xsl:call-template>
			</xsl:with-param>
		</xsl:call-template>
	
		<xsl:copy-of select="$multiline-position-end"/>
		<xsl:value-of select ="name(.)"/>
		<xsl:text>&#10;</xsl:text>
	</xsl:template>

	<xsl:template match="cm:code_block">
		<xsl:choose>
			<xsl:when test="$indented-code">
				<xsl:variable name="content">
					<xsl:call-template name="remove-last">
						<xsl:with-param name="str" select="." />
					</xsl:call-template>
				</xsl:variable>
			
				<xsl:call-template name="general-inline">
					<xsl:with-param name="inline-name" select="'inputVerbatim'"/>
					<xsl:with-param name="inline-content">
						<xsl:call-template name="enclose-hash">
							<xsl:with-param name="hashed-content" select="ext:Hash($content)"/>
						</xsl:call-template>
					</xsl:with-param>
				</xsl:call-template>
			</xsl:when>
			<xsl:otherwise>
				<xsl:copy-of select="$multiline-position-start"/>
				<xsl:text>fencedCode</xsl:text>
				<xsl:text>&#10;</xsl:text>
				
				<xsl:variable name="content">
					<xsl:call-template name="remove-last">
						<xsl:with-param name="str" select="." />
					</xsl:call-template>
				</xsl:variable>
				
				<xsl:call-template name="multiline-attribute">
					<xsl:with-param name="name" select="'src'"/>
					<xsl:with-param name="value">
						<xsl:call-template name="enclose-hash">
							<xsl:with-param name="hashed-content" select="ext:Hash($content)"/>
						</xsl:call-template>
					</xsl:with-param>
				</xsl:call-template>
				
				<xsl:call-template name="multiline-attribute">
					<xsl:with-param name="name" select="'infostring'"/>
					<xsl:with-param name="value">
						<xsl:call-template name="escape-text">
							<xsl:with-param name="text" select="@info"/>
							<xsl:with-param name="inline" select="'true'"/>
							<xsl:with-param name="map-type" select="'programmatic'"/>
						</xsl:call-template>
					</xsl:with-param>
				</xsl:call-template>
				
				<xsl:copy-of select="$multiline-position-end"/>
				<xsl:text>fencedCode</xsl:text>
			</xsl:otherwise>
		</xsl:choose>
		<xsl:text>&#10;</xsl:text>
	</xsl:template>
	
	<xsl:template match="cm:html_block">
		<xsl:call-template name="general-inline">
			<xsl:with-param name="inline-name" select="'inputBlockHtmlElement'"/>
			<xsl:with-param name="inline-content">
				<xsl:call-template name="enclose-hash">
					<xsl:with-param name="hashed-content" select="ext:Hash(.)"/>
				</xsl:call-template>
			</xsl:with-param>
		</xsl:call-template>
	</xsl:template>
	
	<xsl:template match="cm:html_inline">
		<xsl:variable name="content" select="."/>
		
		<xsl:variable name="is-valid-comment">
			<xsl:call-template name="is-inline-html-comment">
				<xsl:with-param name="content" select="$content">
				</xsl:with-param>
			</xsl:call-template>
		</xsl:variable>
		
		<xsl:choose>
			<xsl:when test="$is-valid-comment = 'true'">
				<xsl:variable name="comment-content">
					<xsl:call-template name="extract-comment-content">
						<xsl:with-param name="content" select="$content">
						</xsl:with-param>
					</xsl:call-template>
				</xsl:variable>
					
				<xsl:variable name="is-valid-comment-content">
					<xsl:call-template name="is-valid-comment-content">
						<xsl:with-param name="comment-content" select="$comment-content">
						</xsl:with-param>
					</xsl:call-template>
				</xsl:variable>
					
				<xsl:choose>
					<xsl:when test="$is-valid-comment-content">
						<xsl:call-template name="general-inline">
							<xsl:with-param name="inline-name" select="'inlineHtmlComment'"/>
							<xsl:with-param name="inline-content">
								<xsl:variable name="content-with-replaced-newlines">
									<xsl:call-template name="replace-newlines-with-spaces">
										<xsl:with-param name="text" select="$comment-content"/>
									</xsl:call-template>
								</xsl:variable>

								<xsl:call-template name="escape-text">
									<xsl:with-param name="text" select="$content-with-replaced-newlines"/>
									<xsl:with-param name="inline" select="'true'"/>
									<xsl:with-param name="map-type" select="'typographic'"/>
								</xsl:call-template>
							</xsl:with-param>
						</xsl:call-template>
					</xsl:when>
					<xsl:otherwise>
						<xsl:call-template name="general-inline">
							<xsl:with-param name="inline-name" select="'inlineHtmlTag'"/>
							<xsl:with-param name="inline-content">
								<xsl:variable name="content-with-replaced-newlines">
									<xsl:call-template name="replace-newlines-with-spaces">
										<xsl:with-param name="text" select="."/>
									</xsl:call-template>
								</xsl:variable>

								<xsl:call-template name="escape-text">
									<xsl:with-param name="text" select="$content-with-replaced-newlines"/>
									<xsl:with-param name="inline" select="'true'"/>
									<xsl:with-param name="map-type" select="'typographic'"/>
								</xsl:call-template>
							</xsl:with-param>
						</xsl:call-template>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:when>
			<xsl:otherwise>
				<xsl:call-template name="general-inline">
					<xsl:with-param name="inline-name" select="'inlineHtmlTag'"/>
					<xsl:with-param name="inline-content">
						<xsl:variable name="content-with-replaced-newlines">
							<xsl:call-template name="replace-newlines-with-spaces">
								<xsl:with-param name="text" select="."/>
							</xsl:call-template>
						</xsl:variable>

						<xsl:call-template name="escape-text">
							<xsl:with-param name="text" select="$content-with-replaced-newlines"/>
							<xsl:with-param name="inline" select="'true'"/>
							<xsl:with-param name="map-type" select="'typographic'"/>
						</xsl:call-template>
					</xsl:with-param>
				</xsl:call-template>
			</xsl:otherwise>
		</xsl:choose>

	</xsl:template>

	<!-- SPECIAL CHARACTERS / ESCAPING -->

	<xsl:template name="escape-text">
		<xsl:param name="text"/>
		<xsl:param name="inline"/>
		<xsl:param name="map-type"/>

		<xsl:call-template name="map-special">
			<xsl:with-param name="character" select="substring($text, 1, 1)"/>
			<xsl:with-param name="inline" select="$inline"/>
			<xsl:with-param name="map-type" select="$map-type"/>
		</xsl:call-template>

		<xsl:if test="string-length($text) > 1">
			<xsl:call-template name="escape-text">
				<xsl:with-param name="text" select="substring($text, 2)"/>
				<xsl:with-param name="inline" select="$inline"/>
				<xsl:with-param name="map-type" select="$map-type"/>
			</xsl:call-template>
		</xsl:if>

	</xsl:template>

	<xsl:template name="parenthesise-text">
		<xsl:param name="str" />
		<xsl:param name="inline" />

		<xsl:choose>
			<xsl:when test="$inline = 'true'">
				<xsl:text>(</xsl:text>
				<xsl:value-of select="$str"/>
				<xsl:text>)</xsl:text>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$str"/>
				<xsl:text>&#10;</xsl:text>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template name="parenthesise-group">
		<xsl:param name="str" />

		<xsl:if test="$str != ''">

			<xsl:variable name="first" select="substring-before($str, '&#10;')"/>
			<xsl:variable name="other" select="substring-after($str, '&#10;')"/>

			<xsl:choose>
				<xsl:when test="$first = ''">
					<xsl:value-of select="$str"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:call-template name="parenthesise-text">
						<xsl:with-param name="str" select="$first" />
						<xsl:with-param name="inline" select="'true'"/>
					</xsl:call-template>
					<xsl:call-template name="parenthesise-group">
						<xsl:with-param name="str" select="$other" />
					</xsl:call-template>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:if>
	</xsl:template>

	<xsl:template name="map-special">
		<xsl:param name="character"/>
		<xsl:param name="inline"/>
		<xsl:param name="only-minimal"/>
		<xsl:param name="map-type"/>

		<xsl:variable name="mapped-character">
			<xsl:choose>
				<xsl:when test="$map-type = 'typographic'">
					<xsl:choose>
						<xsl:when test="$character = '#'">hash</xsl:when>
						<xsl:when test="$character = '$'">dollarSign</xsl:when>
						<xsl:when test="$character = '%'">percentSign</xsl:when>
						<xsl:when test="$character = '&amp;'">ampersand</xsl:when>
						<xsl:when test="$character = '\'">backslash</xsl:when>
						<xsl:when test="$character = '^'">circumflex</xsl:when>
						<xsl:when test="$character = '_'">underscore</xsl:when>
						<xsl:when test="$character = '{'">leftBrace</xsl:when>
						<xsl:when test="$character = '}'">rightBrace</xsl:when>
						<xsl:when test="$character = '|'">pipe</xsl:when>
						<xsl:when test="$character = '~'">tilde</xsl:when>
						<xsl:when test="$character = '&#160;'">nbsp</xsl:when>
						<xsl:when test="$inline = 'true'">
							<xsl:value-of select="$character"/>
						</xsl:when>
					</xsl:choose>
				</xsl:when>
				<xsl:when test="$map-type = 'programmatic'">
					<xsl:choose>
						<xsl:when test="$character = '{'">leftBrace</xsl:when>
						<xsl:when test="$character = '}'">rightBrace</xsl:when>
						<xsl:when test="$character = '\'">backslash</xsl:when>
						<xsl:when test="$inline = 'true'">
							<xsl:value-of select="$character"/>
						</xsl:when>
					</xsl:choose>
				</xsl:when>
				<xsl:when test="$inline = 'true'">
					<xsl:value-of select="$character"/>
				</xsl:when>
			</xsl:choose>
		</xsl:variable>

		<xsl:choose>
			<xsl:when test="$mapped-character = ''">
			</xsl:when>
			<xsl:when test="$character = $mapped-character">
				<xsl:value-of select="$character"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:call-template name="parenthesise-text">
					<xsl:with-param name="str" select="$mapped-character"/>
					<xsl:with-param name="inline" select="$inline"/>
				</xsl:call-template>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<!-- EXTENSIONS -->
	
	<!-- TODO SPECIFY AS BLOCK, ADD TO BLOCKS -->
	<xsl:template match="cm:line_block">
		<xsl:variable name="is-enabled">
			<xsl:call-template name="is-extension-enabled">
				<xsl:with-param name="extension-name" select="'line_blocks'" />
				<xsl:with-param name="current-extensions">
					<xsl:copy-of select="$extensions" />
				</xsl:with-param>
			</xsl:call-template>
		</xsl:variable>
		
		<xsl:if test="$is-enabled = 'true()'">
			<xsl:text>lineBlockBegin</xsl:text>
			<xsl:text>&#10;</xsl:text>
			<xsl:apply-templates select="cm:*" />
			<xsl:text>lineBlockEnd</xsl:text>
			<xsl:text>&#10;</xsl:text>
		</xsl:if>
	</xsl:template>

	<!-- TODO 
	
	Formatting - in progress
	
	-->

	<!-- TODO CHANGES SINCE MARKX 1.0 - SECTIONS -->

	<xsl:template match="cm:heading_with_sections">
		<xsl:text>sectionBegin</xsl:text>
		<xsl:text>&#10;</xsl:text>
		<xsl:text>heading</xsl:text>
		<xsl:choose>
			<xsl:when test="@level = 1">One</xsl:when>
			<xsl:when test="@level = 2">Two</xsl:when>
			<xsl:when test="@level = 3">Three</xsl:when>
			<xsl:when test="@level = 4">Four</xsl:when>
			<xsl:when test="@level = 5">Five</xsl:when>
			<xsl:when test="@level = 6">Six</xsl:when>
		</xsl:choose>
		<xsl:copy-of select="$inline-start"/>
		<xsl:call-template name="inline"/>
		<xsl:text>&#10;</xsl:text>
	</xsl:template>

</xsl:stylesheet>
