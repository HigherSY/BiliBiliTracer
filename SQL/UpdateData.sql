CREATE PROCEDURE bili.UpdateFollower(@Data nvarchar(max), @Vmid int)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @Mid int
	SET @Mid = cast(JSON_VALUE(@Data, '$.mid') as int);

    UPDATE bili.Followers SET followerData = @Data, vmid = @Vmid WHERE mid = @Mid AND vmid = @Vmid;
    
	IF(@@ROWCOUNT = 0)
	BEGIN
		INSERT INTO bili.Followers (followerData, vmid)
		VALUES(@Data, @Vmid)
	END
END