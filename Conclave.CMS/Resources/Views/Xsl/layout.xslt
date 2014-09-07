<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
	<xsl:output method="html" indent="yes" />

	<xsl:variable name="params" select="/records/item[@name='params']/records/item" />
	<xsl:variable name="app-path" select="$params[@name='app-path']/@value" />

	<xsl:template match="/">
		<xsl:text disable-output-escaping="yes">&lt;!DOCTYPE html &gt;</xsl:text>
		<html>
			<head>
				<link rel="stylesheet" type="text/css" href="{$app-path}/resources/css/bootstrap.css" />
				<link rel="stylesheet" type="text/css" href="{$app-path}/resources/css/datetimepicker.css"></link>
				<link rel="stylesheet" type="text/css" href="{$app-path}/resources/css/bootstrap-editable.css" />
				<link rel="stylesheet" type="text/css" href="{$app-path}/resources/css/layout.css" />
				
				<script type="text/javascript" src="{$app-path}/resources/js/jquery.js"></script>
				<script type="text/javascript" src="{$app-path}/resources/js/bootstrap.js"></script>
				<script type="text/javascript" src="{$app-path}/resources/js/bootstrap-datetimepicker.js"></script>
				<script type="text/javascript" src="{$app-path}/resources/js/bootstrap-editable.js"></script>
				<script type="text/javascript" src="{$app-path}/resources/js/layout.js"></script>
				<xsl:apply-templates mode="view-head" />
			</head>
			<body>
				<div class="navbar navbar-fixed-top">
					<div class="navbar-inner">
						<div class="container">
							<button type="button" class="btn btn-navbar" data-toggle="collapse" data-target=".nav-collapse">
								<span class="icon-bar"></span>
								<span class="icon-bar"></span>
								<span class="icon-bar"></span>
							</button>
							<a class="brand" href="./index.html">Conclave.io</a>
							<div class="nav-collapse collapse">
								<ul class="nav pull-right">
									<li class="active">
										<a href="./home.html">Development Blog</a>
									</li>
									<li class="">
										<a href="./quizzes.html">Middlesea</a>
									</li>
									<li class="divider-vertical"></li>
									<li class="">
										<a href="./account.html">My Account</a>
									</li>
								</ul>
							</div>
						</div>
					</div>
				</div>
				<div class="view">
					<xsl:apply-templates mode="view" />
				</div>
				<div class="navbar navbar-fixed-bottom">
					<div class="navbar-inner">
						<div class="container">
							<span class="brand">
								processed in: <xsl:value-of select="/records/item[@name='timers']/records/item[@name='process-request']/timer/@duration"/>ms
							</span>
						</div>
					</div>
				</div>
			</body>
		</html>
	</xsl:template>

	<xsl:template match="/records/item[@name='errors']/list">
		<div class="error-messages">
			<xsl:apply-templates select="error[exception]" />
			<xsl:apply-templates select="error[not(exception)]" />
		</div>
	</xsl:template>

	<xsl:template match="/records/item[@name='messages']/list">
		<div class="messages">
			<xsl:apply-templates select="item" />
		</div>
	</xsl:template>

	<xsl:template match="/records/item[@name='errors']/list/error">
		<div class="alert">
			<xsl:choose>
				<xsl:when test="exception">
					<xsl:attribute name="class">alert alert-error</xsl:attribute>
				</xsl:when>
				<xsl:otherwise>
					<xsl:attribute name="class">alert</xsl:attribute>
				</xsl:otherwise>
			</xsl:choose>
			<button type="button" class="close" data-dismiss="alert"><i class="icon-remove">&#32;</i></button>
			<div>
				<xsl:value-of select="@message"/>
			</div>
			<div>
				<small>
					(<xsl:value-of select="exception"/>)
				</small>
			</div>
		</div>
	</xsl:template>

	<xsl:template match="/records/item[@name='messages']/list/item">
		<div class="alert alert-success">
			<button type="button" class="close" data-dismiss="alert">
				<i class="icon-remove" title="dismiss"></i>
			</button>
			<xsl:value-of select="."/>
		</div>
	</xsl:template>
</xsl:stylesheet>
