<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0"
xmlns:cm="http://commonmark.org/xml/1.0" xmlns:ext="mark:ext">
    <xsl:output method="text" omit-xml-declaration="yes" encoding="UTF-8" indent="no"/>

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

	
	<!-- ROOT -->

	<xsl:template match="/">
        <xsl:apply-templates select="cm:document"/>
    </xsl:template>

	<!-- BLOCKS -->

    <xsl:template match="cm:document">
        <xsl:text>documentBegin</xsl:text>
		<xsl:text>&#10;</xsl:text>
        <xsl:apply-templates select="cm:*" />
        <xsl:text>documentEnd</xsl:text>
		<xsl:text>&#10;</xsl:text>
    </xsl:template>

    <xsl:template match="cm:paragraph">
        <xsl:apply-templates select="cm:*"/>
    </xsl:template>

	<xsl:template match="cm:softbreak">
		<xsl:apply-templates select="ancestor::cm:*"/>
		<xsl:text>&#10;</xsl:text>
	</xsl:template>

	<xsl:template match="cm:linebreak">
		<xsl:apply-templates select="ancestor::cm:*"/>
		<xsl:text>&#10;</xsl:text>
	</xsl:template>

	<xsl:template match="cm:thematic_break">
		<xsl:apply-templates select="ancestor::cm:*"/>
		<xsl:text>&#10;</xsl:text>
	</xsl:template>

	<xsl:template name="list-content">
		<xsl:param name="list-type"/>

		<xsl:variable name="tight" select="'Tight'"/>

		<xsl:copy-of select="$list-type"/>
		<xsl:copy-of select="$block-position-start"/>
		<xsl:if test="@tight">
			<xsl:copy-of select="$tight"/>
		</xsl:if>

		<xsl:apply-templates select="cm:*" />
		<xsl:text>&#10;</xsl:text>

		<xsl:copy-of select="$list-type"/>
		<xsl:copy-of select="$block-position-end"/>
		<xsl:if test="@tight">
			<xsl:copy-of select="$tight"/>
		</xsl:if>
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

	<xsl:template match="cm:item">
		<xsl:choose>
			<xsl:when test="../@type = 'bullet'">
				<xsl:text>ul</xsl:text>
				<xsl:text>Item</xsl:text>
				<xsl:apply-templates select="cm:*" />
				<xsl:text>&#10;</xsl:text>
				<xsl:text>ul</xsl:text>
				<xsl:text>Item</xsl:text>
				<xsl:text>End</xsl:text>
			</xsl:when>
			<xsl:when test="../@type = 'ordered'">
				<xsl:text>ol</xsl:text><xsl:text>Item</xsl:text><xsl:text>WithNumber</xsl:text>: <xsl:value-of select="../@start + position() - 1"/>
				<xsl:apply-templates select="cm:*" />
				<xsl:text>&#10;</xsl:text>
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
						<xsl:apply-templates select="." />
					</xsl:when>
					<xsl:otherwise>
						<xsl:text>(</xsl:text>
						<xsl:apply-templates select="." />
						<xsl:text>)</xsl:text>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:for-each>
		</xsl:variable>

		<xsl:value-of select="translate($sub, '&#10;', '')"/>
	</xsl:template>
    
	<xsl:template match="cm:heading">
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

	<xsl:template match="cm:emph">
		<xsl:text>emphasis</xsl:text>
		<xsl:copy-of select="$inline-start"/>
		<xsl:call-template name="inline"/>
		<xsl:text>&#10;</xsl:text>
    </xsl:template>

	<xsl:template match="cm:strong">
		<xsl:text>strongEmphasis</xsl:text>
		<xsl:copy-of select="$inline-start"/>
		<xsl:call-template name="inline"/>
		<xsl:text>&#10;</xsl:text>
	</xsl:template>

	<xsl:template match="cm:text">
		<xsl:value-of select="." />
		<xsl:text>&#10;</xsl:text>
	</xsl:template>

	<xsl:template match="cm:link | cm:image">
		<xsl:copy-of select="$multiline-position-start"/><xsl:value-of select ="name(.)"/><xsl:text>&#10;</xsl:text>
		<xsl:text>- label: </xsl:text><xsl:call-template name="inline"/>
		<xsl:text>&#10;</xsl:text>
		<xsl:text>- URI: </xsl:text><xsl:value-of select ="@destination"/>
		<xsl:text>&#10;</xsl:text>
		<xsl:text>- title: </xsl:text><xsl:value-of select ="@title"/>
		<xsl:text>&#10;</xsl:text>
		<xsl:copy-of select="$multiline-position-end"/><xsl:value-of select ="name(.)"/>
		<xsl:text>&#10;</xsl:text>
	</xsl:template>

	<xsl:template match="cm:code_block">
		<xsl:copy-of select="$multiline-position-start"/><xsl:text>fencedCode</xsl:text>
		<xsl:text>&#10;</xsl:text>
		<xsl:text>- src: </xsl:text>
		<xsl:variable name="content">
			<xsl:call-template name="remove-last">
				<xsl:with-param name="str" select="." />
			</xsl:call-template>
		</xsl:variable>
		<xsl:text>./_markdown_test/</xsl:text><xsl:value-of select ="ext:Hash($content)"/><xsl:text>.verbatim</xsl:text>
		<xsl:text>&#10;</xsl:text>
		<xsl:text>- infostring: </xsl:text>
		<xsl:text>&#10;</xsl:text>
		<xsl:text>End </xsl:text><xsl:text>fencedCode</xsl:text>
		<xsl:text>&#10;</xsl:text>
	</xsl:template>
	
	<xsl:template match="cm:html_block">
		<xsl:text>inputBlockHtmlElement</xsl:text>: <xsl:text>./_markdown_test/</xsl:text><xsl:value-of select ="ext:Hash(.)"/><xsl:text>.verbatim</xsl:text>
		<xsl:text>&#10;</xsl:text>
	</xsl:template>

	<xsl:template match="cm:html_inline">
		
		<xsl:text>&#10;</xsl:text>
	</xsl:template>

	<xsl:template match="cm:code">
		<xsl:text>codeSpan</xsl:text>
		<xsl:copy-of select="$inline-start"/>
		<xsl:call-template name="inline"/>
		<xsl:text>&#10;</xsl:text>
	</xsl:template>

	<!-- SPECIAL CHARACTERS / ESCAPING-->
	
	

	<!-- TODO 
	
	Escaping
	Extensions
	Configurations
	BlockSeparator
	Do not render text in blocks
	
	Formatting
	
	-->


</xsl:stylesheet>
