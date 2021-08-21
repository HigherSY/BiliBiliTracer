CREATE Table bili.Relation (
	id bigint identity primary key,
	createTime datetime default GETDATE(),

	vmid int NOT NULL,
	followerData nvarchar(MAX) NOT NULL,
	mid as cast(JSON_VALUE(followerData, '$.mid') as int) persisted,

	--CONSTRAINT fk_userdata FOREIGN KEY (mid)
 --       REFERENCES bili.UserData (mid),
	CONSTRAINT CK_mids UNIQUE(vmid, mid) WITH (IGNORE_DUP_KEY = ON),
	CONSTRAINT CK_followerData CHECK(ISJSON(followerData)=1),
	index ix_mid (mid),
	index ix_vmid (vmid)
);

CREATE Table bili.TotalFollower (
	logTime datetime default GETDATE(),
	vmid int NOT NULL,
	total int NOT NULL,

	index ix_vmid (vmid)
);

CREATE TABLE bili.UserData (
	mid int primary key,
	updateTime datetime default GETDATE(),

	userCardData nvarchar(MAX),
	--stat nvarchar(MAX),
	--navnum nvarchar(MAX),

	working bit default 0,

	CONSTRAINT CK_userCardData CHECK(ISJSON(userCardData)=1),
	--CONSTRAINT CK_stat CHECK(ISJSON(stat)=1),
	--CONSTRAINT CK_navnum CHECK(ISJSON(navnum)=1)
)