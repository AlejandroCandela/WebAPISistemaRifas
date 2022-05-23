namespace WebAPISistemaRifas.Entidades
{
    public class ParticipantesCartasRifa
    {
        public string IdParticipantes { get; set; }
        public string IdCartas { get; set;}
        public string IdRifa { get; set; }

        public Cartas Cartas { get; set; }
        public Participantes Participantes { get; set; }
        public Rifa Rifa { get; set; }
    }
}
