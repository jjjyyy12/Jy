# Jy
.net core 2.0 �汾��ϵͳ
������Ȩ�޹���
CQRSԭ��ֲ�

����Ŀ¼��Ŀ�ṹ��

BuildingBlocks --�������

	Cache	--����
	
		Jy.Cache	--HttpCache Redis �ĵײ㷽����ICache��ʵ��
		
		Jy.CacheService	--ICache���ϲ��װ
		
		Jy.ICache	--HttpCache Redis �ĵײ㷽��
		
	Index	--����
	
		Jy.IIndex	--�����ĵײ�����ӿ�
		
		Jy.IndexService	--IIndex���ϲ��װ
		
		Jy.SolrIndex	--solr�Ļ���ʵ�֣�ʵ��IIndex
	MessageQueue	--��Ϣ
	
		Jy.IMessageQueue	--��Ϣ�ĵײ�����ӿ�
		
		Jy.Kafka	--kafka��ʵ�֣���Ҫʵ��IMessageQueue��IBigQueueOperation������RdKafka
		
		Jy.CKafka	--kafka��ʵ�֣���Ҫʵ��IMessageQueue��IBigQueueOperation������Confluent.Kafka
		
		Jy.QueueService	--IMessageQueue���ϲ��װ
		
		Jy.RabbitMQ	--RabbitMQ��ʵ�֣���Ҫʵ��IMessageQueue��IQueueOperation
		
	Repositories	--�־û�
	
		Jy.DapperBase	--Dapper�Ļ���ʵ�֣�ʵ��IRepositories
		
		Jy.EntityFramewordCoreBase	--efcore�Ļ���ʵ�֣�ʵ��IRepositories
		
		Jy.IRepositories	--���ݳ־û�������������
		
	Resilience	--�ߵ������
	
		Jy.Resilience.Http	--http�ӿڵ��÷���
		
	Jy.DistributedLock	--�ֲ�ʽ����Redlock���Լ��ֲ�ʽ����ǩ
	
	Jy.DistributedLockHandler	--DistributedLock���ϲ��װ���ֲ�ʽ��װ���࣬��Ҫװ��IProcessMessage
	
	Jy.HealthCheck	--api��HealthCheck�м��
	
	Jy.ILog	--�ײ�log����
	
	Jy.Utility	--�ײ������
	
ConsumerApp	--��Ϣ������

	AuthAdmin	--Ȩ�޹���
	
		Jy.ConsumerAuth	--��Ϣ���������壬����̨
		
		Jy.ProcessMessage	--��Ϣ���ѵķ���ʵ��
		
Services	--�����

	AuthAdmin	--Ȩ�޹���
	
		Identity	--token��ȫ��֤
		
			Jy.MVCAuthorization	--������Դ��������Ҫ���õ�mvcȨ����֤�����jwt����֤Ȩ��
			
			Jy.TokenAuth	--identity server��jwt token server
			
			Jy.TokenService	--�û���¼����֤�û�Ȩ�ޣ�ԭ����ֻ��TokenAuth��identityserver����authadminapi���û���Ȩ��ϵͳ������
			
		Jy.Application	--Ȩ�޹����ҵ���߼�����ʵ��
		
		Jy.AuthAdmin.API	--Ȩ�޹���ҵ��api
		
		Jy.AuthAdmin.SolrIndex	--Ȩ�޹����solr������չʵ��
		
		Jy.Dapper	--Ȩ�޹���dapper����չʵ��
		
		Jy.Domain	--Ȩ�޹������������ӿ�
		
		Jy.EntityFrameworkCore	--Ȩ�޹����efcore��չʵ��
		
	CRM	--crmϵͳ
	
		Jy.CRM.API	--crm��ҵ��api
		
		Jy.CRM.Application	--crm��ҵ���߼�����ʵ��
		
		Jy.CRM.Domain	--crm����������ӿ�
		
		Jy.CRM.EntityFrameworkCore	--crm��efcore��չʵ��
		
	SecKill	--��ɱϵͳ
	
	Jy.AuthService	--api��tokenauth��identityserver������ص��õĹ�����װ
	
	Jy.ServicesKeep	--api�ķ���ע���뷢�֣�����zookeeper��ZooKeeperNetEx
	
WebApp	--webǰ��

	AuthAdmin	--Ȩ�޹���
	
		Jy.MVC	--Ȩ�޹���webǰ��
		