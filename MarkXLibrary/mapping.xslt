<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0"
xmlns:cm="http://commonmark.org/xml/1.0">
    <xsl:output method="text" omit-xml-declaration="yes" encoding="UTF-8" indent="no" />

    <xsl:template match="/">
        <xsl:apply-templates select="cm:document"/>
    </xsl:template>

    <xsl:template match="cm:document">
        <xsl:text>documentBegin</xsl:text><xsl:text>&#10;</xsl:text>
        <xsl:apply-templates select="cm:*" />
        <xsl:text>documentEnd</xsl:text><xsl:text>&#10;</xsl:text>
    </xsl:template>
    
    <xsl:template match="cm:paragraph">
        <xsl:apply-templates select="cm:*"/>
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
        </xsl:choose>: <xsl:apply-templates select="cm:*" />
        <xsl:text>&#10;</xsl:text>
    </xsl:template>
    
    <xsl:template match="cm:text">
        <xsl:value-of select="." />
    </xsl:template>
</xsl:stylesheet>
