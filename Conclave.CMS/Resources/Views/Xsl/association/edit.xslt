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
					associations
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
				<li>
					<a href="{$app-path}/public/metadata/edit.aspx?id={$topic/@id}">Metadata</a>
				</li>
				<li class="dropdown active">
					<a href="#">Associations</a>
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
					<xsl:apply-templates select="/records/item[@name='navigation-view']" mode="assocs" />
				</div>
			</div>
		</div>
	</xsl:template>

	<xsl:template match="item[@name='navigation-view']" mode="assocs">
		<h4>Associations</h4>
		<table class="table">
			<thead>
				<tr>
					<th>label</th>
					<th>type</th>
					<th>role</th>
				</tr>
			</thead>
			<tbody>
				<xsl:apply-templates select="records/item" mode="assocs" />
			</tbody>
		</table>
		<xsl:apply-templates select="records/item/records/item/list/association[@id = $params[@name='assoc']/@value]/metadata" mode="assoc-meta" />
	</xsl:template>

	<xsl:template match="item[@name='navigation-view']/records/item" mode="assocs">
		<xsl:apply-templates select="records/item" mode="assocs" />
	</xsl:template>

	<xsl:template match="item[@name='navigation-view']/records/item/records/item" mode="assocs">
		<xsl:apply-templates select="list/association" mode="assocs" />
	</xsl:template>

	<xsl:template match="item[@name='navigation-view']/records/item/records/item/list/association" mode="assocs">
		<tr>
			<xsl:if test="$params[@name='assoc']/@value = @id">
				<xsl:attribute name="class">info</xsl:attribute>
			</xsl:if>
			<td>
				<a href="?id={@parent}&amp;assoc={@id}">
					<xsl:if test="$params[@name='assoc']/@value = @id"><xsl:attribute name="id">current-meta-label</xsl:attribute></xsl:if>
					<xsl:value-of select="metadata/metadata[@name='label']/@value"/>
				</a>
			</td>
			<td>
				<a
					href="#" class="editable"
					data-pk="{@id}::type"
					data-url="{$app-path}/public/association/inline-update.aspx"
					data-title="Enter value for association type."
				>
					<xsl:value-of select="@type"/>
				</a>
			</td>
			<td>
				<a
					href="#" class="editable"
					data-pk="{@id}::role"
					data-url="{$app-path}/public/association/inline-update.aspx"
					data-title="Enter value for association role."
				>
					<xsl:value-of select="@role"/>
				</a>
			</td>
			<td>
				<form method="post" action="edit.aspx">
					<input type="hidden" name="id" value="{$topic/@id}" />
					<input type="hidden" name="assoc" value="{@id}" />
					<input type="hidden" name="update" value="remove" />
					<button class="btn-link" type="submit">remove</button>
				</form>
			</td>
		</tr>
	</xsl:template>

	<xsl:template match="association/metadata" mode="assoc-meta">
		<h5>metadata</h5>
		<table class="table">
			<thead>
				<tr>
					<th>name</th>
					<th>value</th>
					<th>&#32;</th>
				</tr>
			</thead>
			<tbody>
				<xsl:apply-templates select="metadata" mode="assoc-meta" />
			</tbody>
			<form method="post" action="edit.aspx">
				<input type="hidden" name="id" value="{$topic/@id}" />
				<input type="hidden" name="parent" value="{../@id}" />
				<input type="hidden" name="assoc" value="{../@id}" />
				<input type="hidden" name="scope" value="default" />
				<input type="hidden" name="update" value="add-metadata" />
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

	<xsl:template match="association/metadata/metadata" mode="assoc-meta">
		<tr>
			<td>
				<xsl:value-of select="@name"/>:
			</td>
			<td>
				<a
					href="#" class="editable"
					data-pk="{@for}::{@name}::{@scope}"
					data-url="{$app-path}/public/association/inline-update-metadata.aspx"
					data-title="Enter metadata value for '{@name}'."
					data-update="#current-meta-label"
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
					<input type="hidden" name="assoc" value="{@for}" />
					<input type="hidden" name="parent" value="{@for}" />
					<input type="hidden" name="name" value="{@name}" />
					<input type="hidden" name="scope" value="{@scope}" />
					<input type="hidden" name="update" value="remove-metadata" />
					<button class="btn-link" type="submit">remove</button>
				</form>
			</td>
		</tr>
	</xsl:template>
	

</xsl:stylesheet>
