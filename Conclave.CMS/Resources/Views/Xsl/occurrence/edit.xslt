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
				<small>edit occurrences</small>
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
				<li>
					<a href="{$app-path}/public/association/edit.aspx?id={$topic/@id}">Associations</a>
				</li>
				<li class="active">
					<a href="#">Occurrences</a>
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
					<xsl:apply-templates select="occurrences" mode="view" />
				</div>
			</div>
		</div>
	</xsl:template>

	<xsl:template match="topic/occurrences" mode="view">
		<table class="table">
			<thead>
				<tr>
					<th>role</th>
					<th>behaviour</th>
					<th>reference</th>
					<th>&#32;</th>
				</tr>
			</thead>
			<tbody>
				<xsl:apply-templates select="occurrence" mode="view" />
			</tbody>
			<form method="post" action="edit.aspx">
				<input type="hidden" name="id" value="{$topic/@id}" />
				<input type="hidden" name="parent" value="{$topic/@id}" />
				<input type="hidden" name="scope" value="default" />
				<tfoot>
					<tr>
						<td>
							<input type="text" class="input-small" name="role" placeholder="role" />
						</td>
						<td>
							<input type="text" class="input-small" name="behaviour" placeholder="behaviour" />
						</td>
						<td>
							<div class="input-append">
								<input type="text" class="input-large" name="reference" placeholder="reference" />
								<button class="btn" type="submit">add occurrence</button>
							</div>
						</td>
						<td>&#32;</td>
					</tr>
				</tfoot>
			</form>
		</table>
	</xsl:template>

	<xsl:template match="topic/occurrences/occurrence" mode="view">
		<tr>
			<td>
				<a
					href="#" class="editable"
					data-pk="{@for}::{@scope}::{@role}::{@behaviour}::{@reference}::role"
					data-url="{$app-path}/public/occurrence/inline-update.aspx"
					data-title="Enter occurrence value for 'role'."
					data-type="text"
				>
					<xsl:value-of select="@role"/>
				</a>
			</td>
			<td>
				<a
					href="#" class="editable"
					data-pk="{@for}::{@scope}::{@role}::{@behaviour}::{@reference}::behaviour"
					data-url="{$app-path}/public/occurrence/inline-update.aspx"
					data-title="Enter behaviour value for 'behaviour'."
					data-type="text"
				>
					<xsl:value-of select="@behaviour"/>
				</a>
			</td>
			<td>
				<a
					href="#" class="editable"
					data-pk="{@for}::{@scope}::{@role}::{@behaviour}::{@reference}::reference"
					data-url="{$app-path}/public/occurrence/inline-update.aspx"
					data-title="Enter occurrence value for 'reference'."
					data-type="text"
				>
					<xsl:value-of select="@reference"/>
				</a>
			</td>
			<td>
				<form method="post" action="edit.aspx">
					<input type="hidden" name="id" value="{$topic/@id}" />
					<input type="hidden" name="parent" value="{@for}" />
					<input type="hidden" name="scope" value="{@scope}" />
					<input type="hidden" name="role" value="{@role}" />
					<input type="hidden" name="behaviour" value="{@behaviour}" />
					<input type="hidden" name="reference" value="{@reference}" />
					<input type="hidden" name="update" value="remove" />
					<button class="btn-link" type="submit">remove</button>
				</form>
			</td>
		</tr>
	</xsl:template>


</xsl:stylesheet>
