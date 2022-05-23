using AutoMapper;
using WebAPISistemaRifas.DTOs;
using WebAPISistemaRifas.Entidades;

namespace WebAPISistemaRifas.Extras
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            //Mapeos involucrados con cartas
            CreateMap<CreacionCartasDTO, Cartas>();
            CreateMap<Cartas, CartasDTO>();
            //Mapeos de participantes
            CreateMap<CreacionParticipanteDTO, Participantes>();
            CreateMap<Participantes, ParticipanteDTO>();
            //Mapeos de premios
            CreateMap<CreacionPremioDTO, Premios>();
            CreateMap<PremiosDTO,Premios>();
            //Mapeos de rifa
            CreateMap<Rifa, RifaDTOconCartas>().ForMember(x => x.CartasDTO,
                opciones => opciones.MapFrom(MapCartasVista));
            CreateMap<Rifa, RifaDTOconPremios>().ForMember(x => x.PremiosDTO,
                opciones => opciones.MapFrom(MapPremiosVista));
            //Mapeo necesario para la creacion de la rifa
            CreateMap<CreacionRifaDTO, Rifa>().ForMember(x => x.premios,
                opciones => opciones.MapFrom(MapCreacionRifa));
            CreateMap<RifaPatchDTO, Rifa>().ReverseMap();
            CreateMap<ParticipantesCartasRifa, ParticipantesCartasRifaDTO>();
        }

        private List<Premios> MapCreacionRifa(CreacionRifaDTO creacionRifaDTO, Rifa rifa)
        {
            var lista = new List<Premios>();
            if (creacionRifaDTO.premios == null)
            {
                return lista;
            }

            foreach (var i in creacionRifaDTO.premios)
            {
                lista.Add(new Premios { RifaId = rifa.id, descripcion = i.descripcion, nivel = i.nivel});
            }

            return lista;
        }

        private List<CartasDTO> MapCartasVista(Rifa rifa, RifaDTOconCartas rifaDTOconCartas) 
        {
            var lista = new List<CartasDTO>();
            if (rifa.ParticipantesCartasRifa == null) 
            {
                return lista;
            }
            foreach (var i in rifa.ParticipantesCartasRifa) 
            {
                lista.Add(new CartasDTO { numero = i.Cartas.numero, Nombre = i.Cartas.Nombre });
            }
            return lista;
        }

        private List<PremiosDTO> MapPremiosVista (Rifa rifa, RifaDTOconPremios rifaDTOconPremios)
        {
            var lista = new List<PremiosDTO>();
            if(rifa.premios==null) 
            {
                return lista;
            }
            foreach (var p in rifa.premios) 
            {
                lista.Add(new PremiosDTO { nivel = p.nivel, descripcion = p.descripcion});
            }
            return lista;
        }
    }
}
