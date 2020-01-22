SELECT NumVotaz, GruppoVotaz, Argomento, TipoVotaz, TipoSubVotaz , NListe, SchedaBianca, SchedaNonVoto,
		SchedaContrarioTutte, SchedaAstenutoTutte, SelezTuttiCDA, MaxScelte, MinScelte, PreIntermezzo,
		(select AbilitaBottoneUscita from CONFIG_CfgVotoSegreto) as AbilitaBottoneUscita
 from VS_MatchVot_Totem with (NOLOCK)  where GruppoVotaz < 999 order by NumVotaz 
 