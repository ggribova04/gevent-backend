using AutoMapper;

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

    /*
    CreateMap<TenderDto, Tender>()
    .ForMember(dest => dest.EventId, opt => opt.MapFrom(src => src.EventId))
    .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
    .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
    .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.ServiceName))
    .ForMember(dest => dest.Deadline, opt => opt.MapFrom(src => src.Deadline))
    .ForMember(dest => dest.Contacts, opt => opt.MapFrom(src => src.Contacts))
    .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment))
    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status)) // enum TenderStatus
    .ReverseMap();*/


    // DateOnly <-> DateTime
    CreateMap<DateOnly, DateTime>().ConvertUsing(d => d.ToDateTime(TimeOnly.MinValue));
        CreateMap<DateTime, DateOnly>().ConvertUsing(d => DateOnly.FromDateTime(d));

        // TimeOnly <-> TimeSpan
        CreateMap<TimeOnly, TimeSpan>().ConvertUsing(t => t.ToTimeSpan());
        CreateMap<TimeSpan, TimeOnly>().ConvertUsing(t => TimeOnly.FromTimeSpan(t));

        CreateMap<EventGuest, EventGuestDto>().ReverseMap();
    }
}
