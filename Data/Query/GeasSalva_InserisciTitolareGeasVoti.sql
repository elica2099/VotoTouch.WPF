declare @ProgMozione int

set @ProgMozione = (SELECT GEAS_MatchVot.ProgMozione from GEAS_MatchVot 
					INNER JOIN VS_MatchVot_Gruppo_Totem ON VS_MatchVot_Gruppo_Totem.MozioneRealeGeas = GEAS_MatchVot.MozioneReale
					WHERE VS_MatchVot_Gruppo_Totem.NumSubVotaz = @NumSubVotaz)

IF NOT EXISTS ( SELECT 1 FROM GEAS_Voti WHERE ProgMozione = @ProgMozione and Badge = @Badge )
BEGIN

	INSERT INTO Geas_Voti with (ROWLOCK) (ProgMozione, ProgSubVotaz, Reale, Badge, MozioneRea, SubVotaz, TipoVoto, DataOraVoto, NumFav, IsSelection) 
	SELECT GEAS_MatchVot.ProgMozione, -1, 1, @Badge, 0, 0, 6, dateadd(millisecond, -datepart(millisecond, Getdate()), Getdate()), 0, 0
	FROM VS_MatchVot_Gruppo_Totem 
		INNER JOIN GEAS_MatchVot ON VS_MatchVot_Gruppo_Totem.MozioneRealeGeas = GEAS_MatchVot.MozioneReale 
	WHERE VS_MatchVot_Gruppo_Totem.NumSubVotaz = @NumSubVotaz

END
ELSE
BEGIN
	UPDATE GEAS_Voti set DataOraVoto = dateadd(millisecond, -datepart(millisecond, Getdate()), Getdate())
	WHERE ProgMozione = @ProgMozione and Badge = @Badge
END