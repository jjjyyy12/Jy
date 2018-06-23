# Jy
.net core 2.0 版本的系统
包含：权限管理
CQRS原则分层

具体目录项目结构：

BuildingBlocks --公共组件

	Cache	--缓存
	
		Jy.Cache	--HttpCache Redis 的底层方法，ICache的实现
		
		Jy.CacheService	--ICache的上层封装
		
		Jy.ICache	--HttpCache Redis 的底层方法
		
	Index	--索引
	
		Jy.IIndex	--索引的底层操作接口
		
		Jy.IndexService	--IIndex的上层封装
		
		Jy.SolrIndex	--solr的基础实现，实现IIndex
	MessageQueue	--消息
	
		Jy.IMessageQueue	--消息的底层操作接口
		
		Jy.Kafka	--kafka的实现，主要实现IMessageQueue的IBigQueueOperation，基于RdKafka
		
		Jy.CKafka	--kafka的实现，主要实现IMessageQueue的IBigQueueOperation，基于Confluent.Kafka
		
		Jy.QueueService	--IMessageQueue的上层封装
		
		Jy.RabbitMQ	--RabbitMQ的实现，主要实现IMessageQueue的IQueueOperation
		
	Repositories	--持久化
	
		Jy.DapperBase	--Dapper的基础实现，实现IRepositories
		
		Jy.EntityFramewordCoreBase	--efcore的基础实现，实现IRepositories
		
		Jy.IRepositories	--数据持久化基础操作方法
		
	Resilience	--高弹性组件
	
		Jy.Resilience.Http	--http接口调用方法
		
	Jy.DistributedLock	--分布式锁，Redlock，以及分布式锁标签
	
	Jy.DistributedLockHandler	--DistributedLock的上层封装，分布式锁装饰类，主要装饰IProcessMessage
	
	Jy.HealthCheck	--api的HealthCheck中间件
	
	Jy.ILog	--底层log方法
	
	Jy.Utility	--底层帮助类
	
ConsumerApp	--消息消费者

	AuthAdmin	--权限管理
	
		Jy.ConsumerAuth	--消息消费者载体，控制台
		
		Jy.ProcessMessage	--消息消费的方法实现
		
Services	--服务层

	AuthAdmin	--权限管理
	
		Identity	--token安全认证
		
			Jy.MVCAuthorization	--调用资源服务器需要引用的mvc权限验证组件，jwt的验证权限
			
			Jy.TokenAuth	--identity server，jwt token server
			
			Jy.TokenService	--用户登录、验证用户权限，原则上只被TokenAuth（identityserver），authadminapi（用户，权限系统）引用
			
		Jy.Application	--权限管理的业务逻辑具体实现
		
		Jy.AuthAdmin.API	--权限管理业务api
		
		Jy.AuthAdmin.SolrIndex	--权限管理的solr索引拓展实现
		
		Jy.Dapper	--权限管理dapper的拓展实现
		
		Jy.Domain	--权限管理的领域类与接口
		
		Jy.EntityFrameworkCore	--权限管理的efcore拓展实现
		
	CRM	--crm系统
	
		Jy.CRM.API	--crm的业务api
		
		Jy.CRM.Application	--crm的业务逻辑具体实现
		
		Jy.CRM.Domain	--crm的领域类与接口
		
		Jy.CRM.EntityFrameworkCore	--crm的efcore拓展实现
		
	SecKill	--秒杀系统
	
	Jy.AuthService	--api与tokenauth（identityserver）的相关调用的公共封装
	
	Jy.ServicesKeep	--api的服务注册与发现，基于zookeeper，ZooKeeperNetEx
	
WebApp	--web前端

	AuthAdmin	--权限管理
	
		Jy.MVC	--权限管理web前端
		