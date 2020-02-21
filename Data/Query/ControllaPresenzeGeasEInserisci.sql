

DECLARE @MozioneReale as int
DECLARE @ProgMozione as int
DECLARE @ApVotDate as datetime
DECLARE @ValAssem as VarChar(3)

if @Votodigruppo = 1
begin
	set @MozioneReale = (select top 1 isnull(VS_MatchVot_Gruppo_Totem.MozioneRealeGeas, -1) as MozioneReale 
					from VS_MatchVot_Gruppo_Totem
					where VS_MatchVot_Gruppo_Totem.NumVotaz = @NumVotaz)
end
else
begin
	set @MozioneReale = (select top 1 isnull(VS_MatchVot_Totem.MozioneRealeGeas, -1) as MozioneReale  
					from VS_MatchVot_Totem 
					where VS_MatchVot_Totem.NumVotaz = @NumVotaz) 
end

set @ProgMozione = (select isnull(GEAS_MatchVot.ProgMozione, 0) as ProgMozione from GEAS_MatchVot 
					where GEAS_MatchVot.MozioneReale = @MozioneReale) 
set @ApVotDate = (select isnull(GEAS_Eventi.DataOra, 0) as ApVotDate from GEAS_Eventi 
					where GEAS_Eventi.ProgMozione = @ProgMozione and GEAS_Eventi.CodEvento = '(APVO)') 
set @ValAssem = (select isnull(GEAS_MatchVot.TipoAsse, 0) as TipoAssemblea from GEAS_MatchVot 
					where GEAS_MatchVot.MozioneReale = @MozioneReale)

if @ValAssem = 'S'
begin
	set @ValAssem = 'O'
end
else
begin
	set @ValAssem = 'S'	
end

DECLARE	@return_value int

EXEC	@return_value = SP_BadgePresente @ApVotDate, @Badge, @ValAssem
set @return_value = (SELECT @@ROWCOUNT )

if @return_value = 0
begin
	insert into Geas_TimbinOut with (ROWLOCK) 
		(DataOra, Badge, TipoMov, Reale, Classe, Terminale, DataIns)
		values (DATEADD(SECOND, -1, @ApVotDate), @Badge, 'E', 1, 99, @Sala, { fn NOW() })
	insert into Geas_TimbinOut with (ROWLOCK) 
		(DataOra, Badge, TipoMov, Reale, Classe, Terminale, DataIns)
		values (DATEADD(SECOND, 1, @ApVotDate), @Badge, 'U', 1, 99, @Sala, { fn NOW() })
end

