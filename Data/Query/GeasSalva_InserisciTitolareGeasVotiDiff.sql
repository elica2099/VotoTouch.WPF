declare @ProgMozione int

set @ProgMozione = (SELECT GEAS_MatchVot.ProgMozione from GEAS_MatchVot 
					INNER JOIN VS_MatchVot_Gruppo_Totem ON VS_MatchVot_Gruppo_Totem.MozioneRealeGeas = GEAS_MatchVot.MozioneReale
					WHERE VS_MatchVot_Gruppo_Totem.NumSubVotaz = @NumSubVotaz)

IF NOT EXISTS ( SELECT 1 FROM GEAS_Voti WHERE ProgMozione = @ProgMozione and Badge = @Badge )
BEGIN

	INSERT INTO Geas_VotiDiff with (ROWLOCK) (ProgMozione, ProgSubVotaz, Badge, ProgDeleg, ValAssem, TipoVoto, 
												AzioniSi, VotiSi, PercSi, 
												AzioniNo, VotiNo, PercNo, 
												AzioniAst, VotiAst, PercAst,
												AzioniCi, VotiCi,
												AzioniNv, VotiNv, PercNv, 
												AzioniNq, VotiNq, PercNq, Revocato) 
	SELECT GEAS_MatchVot.ProgMozione, -1, @Badge, 0, @ValAssem, 0, 
									0, 0, 0, 
									0, 0, 0, 
									0, 0, 0, 
									0, 0, 
									0, 0, 0, 
									0, 0, 0, 0
	FROM VS_MatchVot_Gruppo_Totem 
		INNER JOIN GEAS_MatchVot ON VS_MatchVot_Gruppo_Totem.MozioneRealeGeas = GEAS_MatchVot.MozioneReale 
	WHERE VS_MatchVot_Gruppo_Totem.NumSubVotaz = @NumSubVotaz

END
