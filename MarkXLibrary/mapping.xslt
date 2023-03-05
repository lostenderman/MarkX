<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0"
xmlns:cm="http://commonmark.org/xml/1.0" xmlns:func="ext:hash">
    <xsl:output method="text" omit-xml-declaration="yes" encoding="UTF-8" indent="no"/>

	<xsl:variable name="multilineStart" select="'BEGIN '" />
	<xsl:variable name="BlockSeparator" select="'interblockSeparator'"/>
	<xsl:variable name="BlockPositionStart" select="'Begin'"/>
	<xsl:variable name="BlockPositionEnd" select="'End'"/>
	<xsl:variable name="MultilinePositionStart" select="'BEGIN '"/>
	<xsl:variable name="MultilinePositionEnd" select="'END '"/>
	<xsl:variable name="MultilineAttributeStart" select="'- '"/>
	<xsl:variable name="InlineStart" select="': '"/>
	<xsl:variable name="VerbatimDirectory" select="'./_markdown_test/'"/>
	<xsl:variable name="Verbatim" select="'.verbatim'"/>

	<xsl:template match="/">
        <xsl:apply-templates select="cm:document"/>
    </xsl:template>

    <xsl:template match="cm:document">
        <xsl:text>documentBegin</xsl:text>
        <xsl:apply-templates select="cm:*" />
		<xsl:text>&#10;</xsl:text>
        <xsl:text>documentEnd</xsl:text>
    </xsl:template>

	<xsl:variable name="test">
		<xsl:call-template name="inline" />
	</xsl:variable>

	<xsl:template name="inline">
		<xsl:apply-templates select="cm:*" />
	</xsl:template>
	
    <xsl:template match="cm:paragraph">
        <xsl:apply-templates select="cm:*"/>
    </xsl:template>
    
	<xsl:template match="cm:heading">
		<xsl:text>&#10;</xsl:text>
        <xsl:text>heading</xsl:text>
        <xsl:choose>
            <xsl:when test="@level = 1">One</xsl:when>
            <xsl:when test="@level = 2">Two</xsl:when>
            <xsl:when test="@level = 3">Three</xsl:when>
            <xsl:when test="@level = 4">Four</xsl:when>
            <xsl:when test="@level = 5">Five</xsl:when>
        	<xsl:when test="@level = 6">Six</xsl:when>
        </xsl:choose>: <xsl:apply-templates select="cm:*" />
    </xsl:template>

	<xsl:template match="cm:emph">
		<xsl:variable name="sub">
			<xsl:apply-templates select="cm:text" />
			(<xsl:apply-templates select="cm:* [name(  ) != 'text']" />)
		</xsl:variable>
		<xsl:text>&#10;</xsl:text>
		<xsl:text>emphasis</xsl:text>: <xsl:value-of select="normalize-space($sub)"/>
    </xsl:template>

	<xsl:template match="cm:strong">
		<xsl:text>&#10;</xsl:text>
		<xsl:text>strongEmphasis</xsl:text>: <xsl:apply-templates select="cm:*" />
	</xsl:template>

	<xsl:template match="cm:text">
		<xsl:text>&#10;</xsl:text>
		<xsl:value-of select="." />
	</xsl:template>

	<xsl:template match="cm:softbreak">
		<xsl:text>&#10;</xsl:text>
		<xsl:apply-templates select="ancestor::cm:*"/>
	</xsl:template>

	<xsl:template match="cm:linebreak">
		<xsl:text>&#10;</xsl:text>
		<xsl:apply-templates select="ancestor::cm:*"/>
	</xsl:template>

	<xsl:template match="cm:link | cm:image">
		<xsl:text>&#10;</xsl:text>
		<xsl:text>BEGIN </xsl:text><xsl:value-of select ="name(.)"/><xsl:text>&#10;</xsl:text>
		<xsl:text>- label: </xsl:text><xsl:text>&#10;</xsl:text>
		<xsl:text>- URI: </xsl:text><xsl:value-of select ="@destination"/><xsl:text>&#10;</xsl:text>
		<xsl:text>- title: </xsl:text><xsl:value-of select ="@title"/><xsl:text>&#10;</xsl:text>
		<xsl:text>END </xsl:text><xsl:value-of select ="name(.)"/>
	</xsl:template>

	<xsl:template match="cm:code_block">
		<xsl:text>&#10;</xsl:text>
		<xsl:copy-of select="$multilineStart"/><xsl:text>fencedCode</xsl:text><xsl:text>&#10;</xsl:text>
		<xsl:text>- src: </xsl:text><xsl:text>./_markdown_test/</xsl:text><xsl:value-of select ="func:Hash(.)"/><xsl:text>.verbatim</xsl:text><xsl:text>&#10;</xsl:text>
		<xsl:text>- infostring: </xsl:text><xsl:text>&#10;</xsl:text>
		<xsl:text>End </xsl:text><xsl:text>fencedCode</xsl:text>
	</xsl:template>
	
	<xsl:template match="cm:html_block">
		<xsl:text>&#10;</xsl:text>
		<xsl:text>inputBlockHtmlElement</xsl:text>: <xsl:text>./_markdown_test/</xsl:text><xsl:value-of select ="func:Hash(.)"/><xsl:text>.verbatim</xsl:text><xsl:text>&#10;</xsl:text>
	</xsl:template>

	<xsl:template match="cm:html_inline">
		<xsl:text>&#10;</xsl:text>
	</xsl:template>

	<xsl:template match="cm:code">
		<xsl:text>&#10;</xsl:text>
		<xsl:text>codeSpan</xsl:text>: <xsl:apply-templates select="cm:*"/>
	</xsl:template>

	<xsl:template match="cm:thematic_break">
		<xsl:text>&#10;</xsl:text>
		<xsl:apply-templates select="ancestor::cm:*"/>
	</xsl:template>

	<xsl:template match="cm:list">
		<xsl:text>&#10;</xsl:text>
		<xsl:choose>
			<xsl:when test="@type = 'bullet'">
				<xsl:text>ul</xsl:text>
				<xsl:text>Begin</xsl:text>
				<xsl:if test="@tight">Tight</xsl:if>
				<xsl:apply-templates select="cm:*" />
				<xsl:text>&#10;</xsl:text>
				<xsl:text>ul</xsl:text>
				<xsl:text>End</xsl:text>
				<xsl:if test="@tight = true">Tight</xsl:if>
			</xsl:when>
			<xsl:when test="@type = 'ordered'">
				<xsl:text>ol</xsl:text>
				<xsl:text>Begin</xsl:text>
				<xsl:if test="@tight">Tight</xsl:if>
				<xsl:apply-templates select="cm:*" />
				<xsl:text>&#10;</xsl:text>
				<xsl:text>ol</xsl:text>
				<xsl:text>End</xsl:text>
				<xsl:if test="@tight = true">Tight</xsl:if>
			</xsl:when>
		</xsl:choose>
	</xsl:template>

	<xsl:template match="cm:item">
		<xsl:text>&#10;</xsl:text>
		<xsl:choose>
			<xsl:when test="../@type = 'bullet'">
				<xsl:text>ul</xsl:text><xsl:text>Item</xsl:text>
				<xsl:apply-templates select="cm:*" />
				<xsl:text>&#10;</xsl:text>
				<xsl:text>ul</xsl:text><xsl:text>Item</xsl:text><xsl:text>End</xsl:text>
			</xsl:when>
			<xsl:when test="../@type = 'ordered'">
				<xsl:text>ol</xsl:text><xsl:text>Item</xsl:text><xsl:text>WithNumber</xsl:text>: <xsl:value-of select="../@start + position() - 1"/>
				<xsl:apply-templates select="cm:*" />
				<xsl:text>&#10;</xsl:text>
				<xsl:text>ol</xsl:text><xsl:text>Item</xsl:text><xsl:text>End</xsl:text>
			</xsl:when>
		</xsl:choose>
	</xsl:template>

	<!-- TODO -->
	
	<!--
	
	Escaping
	Parenthesising
	Extensions
	Newline stripping
	
	-->


</xsl:stylesheet>
