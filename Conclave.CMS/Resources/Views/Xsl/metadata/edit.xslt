<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>

	<xsl:include href="../layout.xslt"/>

	<xsl:variable name="topic" select="/records/item[@name='topic']/topic" />
	<xsl:variable name="meta" select="$topic/metadata/metadata" />

	<xsl:template  match="@* | node()" mode="view"></xsl:template>
	<xsl:template  match="@* | node()" mode="view-head"></xsl:template>
	<xsl:template  match="@* | node()" mode="view-navigation"></xsl:template>


	<xsl:template match="records" mode="view-head">
		<link rel="stylesheet" href="{$app-path}/resources/css/public/topic/view.css" />
		<title>Some Title</title>
	</xsl:template>

	<xsl:template match="item[@name='navigation-view']" mode="view-navigation">
		<h4>Relations</h4>
		<ul>
			<xsl:apply-templates select="records/item" mode="view-navigation" />
		</ul>
	</xsl:template>

	<xsl:template match="item[@name='navigation-view']/records/item" mode="view-navigation">
		<li>
			<xsl:value-of select="@name"/>
			<ul>
				<xsl:apply-templates select="records/item" mode="view-navigation" />
			</ul>
		</li>
	</xsl:template>

	<xsl:template match="item[@name='navigation-view']/records/item/records/item" mode="view-navigation">
		<li>
			<xsl:value-of select="@name"/>
			<ul>
				<xsl:apply-templates select="list/association" mode="view-navigation" />
			</ul>
		</li>
	</xsl:template>

	<xsl:template match="item[@name='navigation-view']/records/item/records/item/list/association" mode="view-navigation">
		<li>
			<a href="?id={@reference}">
				<xsl:value-of select="metadata/metadata[@name='label']/@value"/>
			</a>
		</li>
	</xsl:template>

	<!-- ENTRY POINT FROM LAYOUT -->
	<xsl:template match="records" mode="view">
		<xsl:apply-templates select="item[@name='topic']/topic" mode="view" />
	</xsl:template>

	<xsl:template match="topic" mode="view">
		<div class="hero-unit">
			<h1>
				<xsl:value-of select="$meta[@name='label']/@value"/>
				<small style="margin-left: 0.5em;">
					metadata
				</small>
			</h1>
		</div>

		<div class="container">
			<ul class="nav nav-tabs">
				<li>
					<a href="{$app-path}/public/topic/view.aspx?id=home">Home</a>
				</li>
				<li>
					<a href="{$app-path}/public/topic/view.aspx?id={$topic/@id}">View</a>
				</li>
				<li>
					<a href="{$app-path}/public/topic/edit.aspx?id={$topic/@id}">Edit</a>
				</li>
				<li class="divider-vertical"></li>
				<li class="active">
					<a href="#">Metadata</a>
				</li>
				<li>
					<a href="{$app-path}/public/association/edit.aspx?id={$topic/@id}">Associations</a>
				</li>
				<li>
					<a href="{$app-path}/public/occurrence/edit.aspx?id={$topic/@id}">Occurrences</a>
				</li>
				<li class="divider-vertical"></li>
				<li class="dropdown">
					<a
						href="#"
						class="dropdown-toggle"
						data-toggle="dropdown"
					>
						Create <b class="caret"></b>
					</a>
					<ul class="dropdown-menu">
						<li>
							<a href="{$app-path}/public/topic/create.aspx?id={$topic/@id}">Topic</a>
						</li>
						<li>
							<a href="{$app-path}/public/association/create.aspx?id={$topic/@id}">Association</a>
						</li>
					</ul>
				</li>
				<li>
					<a href="{$app-path}/public/topic/remove.aspx?id={$topic/@id}">Remove</a>
				</li>
			</ul>
		</div>

		<div class="container">
			<div class="row">
				<div class="span3">
					<xsl:apply-templates select="/records/item[@name='errors']/list" />
					<xsl:apply-templates select="/records/item[@name='messages']/list" />
					<xsl:apply-templates select="/records/item[@name='navigation-view']" mode="view-navigation" />
				</div>
				<div class="span9">
					<xsl:apply-templates select="metadata" mode="view" />
				</div>
			</div>
		</div>
	</xsl:template>

	<xsl:template match="topic/metadata" mode="view">
		<table class="table">
			<thead>
				<tr>
					<th>name</th>
					<th>value</th>
					<th>&#32;</th>
				</tr>
			</thead>
			<tbody>
				<xsl:apply-templates select="metadata" mode="view" />
			</tbody>
			<form method="post" action="edit.aspx">
				<input type="hidden" name="id" value="{$topic/@id}" />
				<input type="hidden" name="parent" value="{$topic/@id}" />
				<input type="hidden" name="scope" value="default" />
				<tfoot>
					<tr>
						<td>
							<input type="text" class="input-small" name="name" placeholder="name" />
						</td>
						<td>
							<div class="input-append">
								<input type="text" class="input-large" name="value" placeholder="value" />
								<button class="btn" type="submit">add metadata</button>
							</div>
						</td>
						<td>&#32;</td>
					</tr>
				</tfoot>
			</form>
		</table>
	</xsl:template>

	<xsl:template match="topic/metadata/metadata" mode="view">
		<tr>
			<td>
				<xsl:value-of select="@name"/>:
			</td>
			<td>
				<a
					href="#" class="editable"
					data-pk="{@for}::{@name}::{@scope}"
					data-url="{$app-path}/public/metadata/inline-update.aspx"
					data-title="Enter metadata value for '{@name}'."
				>
					<xsl:attribute name="data-type">
						<xsl:choose>
							<xsl:when test="@name='description'">textarea</xsl:when>
							<xsl:when test="@name='keywords'">textarea</xsl:when>
							<xsl:when test="@name='edited-on'">datetime</xsl:when>
							<xsl:otherwise>text</xsl:otherwise>
						</xsl:choose>
					</xsl:attribute>
					<xsl:value-of select="@value"/>
				</a>
			</td>
			<td>
				<form method="post" action="edit.aspx">
					<input type="hidden" name="id" value="{$topic/@id}" />
					<input type="hidden" name="parent" value="{@for}" />
					<input type="hidden" name="name" value="{@name}" />
					<input type="hidden" name="scope" value="{@scope}" />
					<input type="hidden" name="update" value="remove" />
					<button class="btn-link" type="submit">remove</button>
				</form>
			</td>
		</tr>
	</xsl:template>


</xsl:stylesheet>
