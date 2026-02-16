using AutoMapper;
using gevent.Database.Enums;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<User, UserDto>().ReverseMap();

        CreateMap<EventDto, Event>()
           .ForMember(dest => dest.OrganizerId, opt => opt.MapFrom(src => src.OrganizerId))
           .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status)) // enum EventStatus
           .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
           .ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.Time))
           .ReverseMap();

        CreateMap<TaskDto, Task>()
          .ForMember(dest => dest.EventId, opt => opt.MapFrom(src => src.EventId))
          .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EmployeeId))
          .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (src.Status))) // string -> enum
          .ReverseMap()
          .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString())); // enum -> string

        // Tender <-> TenderDto
        CreateMap<Tender, TenderDto>()
            .ForMember(dest => dest.EventTitle, opt => opt.MapFrom(src => src.Event.Title))
            .ForMember(dest => dest.EventDate, opt => opt.MapFrom(src => src.Event.Date))
            .ForMember(dest => dest.ViewStatus, opt => opt.Ignore()) // ViewStatus вычисляется в сервисе
            .ReverseMap()
            .ForMember(dest => dest.Event, opt => opt.Ignore()) // Event не маппим обратно
            .ForMember(dest => dest.Responses, opt => opt.Ignore()); // Responses тоже игнорируем

        // CreateTenderRequest -> Tender
        CreateMap<CreateTenderRequest, Tender>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => TenderState.Open)); // при создании всегда Open

        // CreateTenderResponseRequest -> TenderResponse
        CreateMap<CreateTenderResponseRequest, TenderResponse>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => TenderResponseState.Submitted)); // новый отклик всегда Submitted

        // DateOnly <-> DateTime
        CreateMap<DateOnly, DateTime>().ConvertUsing(d => d.ToDateTime(TimeOnly.MinValue));
                CreateMap<DateTime, DateOnly>().ConvertUsing(d => DateOnly.FromDateTime(d));

        // TimeOnly <-> TimeSpan
        CreateMap<TimeOnly, TimeSpan>().ConvertUsing(t => t.ToTimeSpan());
        CreateMap<TimeSpan, TimeOnly>().ConvertUsing(t => TimeOnly.FromTimeSpan(t));

        CreateMap<EventGuest, EventGuestDto>().ReverseMap();
    }
}
