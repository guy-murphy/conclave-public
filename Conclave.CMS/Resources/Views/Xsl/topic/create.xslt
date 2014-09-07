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
		<link rel="stylesheet" href="{$app-path}/resources/css/public/topic/edit.css" />
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

	<xsl:template match="records" mode="view">
		<div class="hero-unit">
			<h1>
				Create Topic
				<small>Where little baby topics begin.</small>
			</h1>
		</div>
		
		<div class="container">
			<ul class="nav nav-tabs">
				<li>
					<a href="{$app-path}/public/topic/view.aspx?id=home">Home</a>
				</li>
				<li class="dropdown active">
					<a
						href="#"
						class="dropdown-toggle"
						data-toggle="dropdown"
					>
						Create <b class="caret"></b>
					</a>
					<ul class="dropdown-menu">
						<li class="active">
							<a href="#">Topic</a>
						</li>
						<li>
							<a href="{$app-path}/public/association/create.aspx?id={$topic/@id}">Association</a>
						</li>
					</ul>
				</li>
			</ul>
		</div>
		
		<div class="container">
            <div class="row">
                <div class="span6 col left-col">
					<xsl:apply-templates select="/records/item[@name='errors']/list" />
					<xsl:apply-templates select="/records/item[@name='messages']/list" />
					<h4>Topic Identity</h4>
					<p>
						Each topic has a unique identity (ID) which is can be refered by.
						The system will generate a long random identity for you, but you can
						and should overight this with a more human readable identity
						such as <emph>FrontPage</emph> where appropriate.
					</p>
					<h4>Topic Label</h4>
					<p>
						A label will be generated from the ID you type if possible, or you may enter a label
						of your own choosing which will be used for the current language and scope of the topic.
						More details control of labels can be achieved by editing the <a href="#">topic metadata</a>.
					</p>
					<h4>Topic Description and Keywords</h4>
					<p>
						Similar to the topic label, a description and keywords for the topic may be entered and will
						be applied for the current language and scope, or again you can get more fine grained control
						from the <a href="#">topic metadata</a>.
					</p>
                </div>
                <div class="span6 col center-col">
                    <form method="post" action="{$app-path}/public/topic/create.aspx" class="form-horizontal">
                        <fieldset>
                            <legend>Topic details.</legend>
                            <div class="control-group">
                                <label for="id" class="control-label">ID</label>
                                <div class="controls">
									<xsl:choose>
										<xsl:when test="$topic">
											<input type="text" class="span4"  name="id" value="{$topic/@id}" />
										</xsl:when>
										<xsl:otherwise>
											<input type="text" class="span4"  name="id" value="{/records/item[@name='new-topic-id']/text-data}" />
										</xsl:otherwise>
									</xsl:choose>
                                </div>
                            </div>
							<div class="control-group">
								<label for="label" class="control-label">Label</label>
								<div class="controls">
									<input type="text" name="label" class="span4" value="{$topic/metadata/metadata[@name='label']/@value}"  />
								</div>
							</div>
							<div class="control-group">
								<label for="description" class="control-label">Description</label>
								<div class="controls">
									<textarea name="description" class="span4"  style="height: 5em">
										<xsl:value-of select="$topic/metadata/metadata[@name='description']/@value"/>
									</textarea>
								</div>
							</div>
							<div class="control-group">
								<label for="keywords" class="control-label">Keywords</label>
								<div class="controls">
									<textarea name="keywords" class="span4" style="height: 5em">
										<xsl:value-of select="$topic/metadata/metadata[@name='keywords']/@value"/>
									</textarea>
								</div>
							</div>
							<div class="control-group">
								<div class="controls">
									<input type="submit" class="span4 btn-large btn-primary" value="create" />
								</div>
							</div>
                        </fieldset>
                    </form>
                </div>
            </div>
        </div>
	</xsl:template>

</xsl:stylesheet>
