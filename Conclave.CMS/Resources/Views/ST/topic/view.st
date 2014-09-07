		<html>
			<head>
				<link rel="stylesheet" type="text/css" href="#params.app_path#/resources/css/bootstrap.css" />
				<link rel="stylesheet" type="text/css" href="#params.app_path#/resources/css/datetimepicker.css"></link>
				<link rel="stylesheet" type="text/css" href="#params.app_path#/resources/css/bootstrap-editable.css" />
				<link rel="stylesheet" type="text/css" href="#params.app_path#/resources/css/layout.css" />
				
				<script type="text/javascript" src="#params.app_path#/resources/js/jquery.js"></script>
				<script type="text/javascript" src="#params.app_path#/resources/js/bootstrap.js"></script>
				<script type="text/javascript" src="#params.app_path#/resources/js/bootstrap-datetimepicker.js"></script>
				<script type="text/javascript" src="#params.app_path#/resources/js/bootstrap-editable.js"></script>
				<script type="text/javascript" src="#params.app_path#/resources/js/layout.js"></script>		
				
				<link rel="stylesheet" href="#params.app_path#/resources/css/public/topic/view.css" />
				<title>Some Title</title>	
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

				
					
					<div class="hero-unit">
						#model.topic.metadata.count#
					</div>
					
				</div>
				<div class="navbar navbar-fixed-bottom">
					<div class="navbar-inner">
						<div class="container">
							<span class="brand">
								processed in: #timers.work.duration#s
							</span>
						</div>
					</div>
				</div>
			</body>
		</html>