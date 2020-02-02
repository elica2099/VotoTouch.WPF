SELECT NumVotaz, MozioneRealeGeas, GruppoVotaz, Argomento, TipoVotaz, TipoSubVotaz , NListe,
		SchedaContrarioTutte, SchedaAstenutoTutte, SelezTuttiCDA, MaxScelte, MinScelte,
		(select VotoBottoneUscita from CONFIG_CfgVotoSegreto) as VotoBottoneUscita,
		(select VotoSchedaBianca from CONFIG_CfgVotoSegreto) as VotoSchedaBianca,
		(select VotoNonVotante from CONFIG_CfgVotoSegreto) as VotoNonVotante
 from VS_MatchVot_Totem with (NOLOCK)  where GruppoVotaz < 999 order by NumVotaz 
 