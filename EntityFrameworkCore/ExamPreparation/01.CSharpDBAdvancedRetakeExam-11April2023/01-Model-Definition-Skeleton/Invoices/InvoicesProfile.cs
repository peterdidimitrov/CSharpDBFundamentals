using AutoMapper;
using Invoices.Data.Models;
using Invoices.DataProcessor.ExportDto;
using System.Globalization;

namespace Invoices
{
    public class InvoicesProfile : Profile
    {
        public InvoicesProfile()
        {
            CreateMap<Invoice, ExportInvoiceDto>()
                .ForMember(dest => dest.InvoiceNumber, opt =>
                    opt.MapFrom(s => s.Number))
                .ForMember(dest => dest.InvoiceAmount, opt =>
                    opt.MapFrom(s => s.Amount))
                .ForMember(dest => dest.DueDate, opt =>
                    opt.MapFrom(s => s.DueDate.ToString("d", CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.Currency, opt =>
                    opt.MapFrom(s => s.CurrencyType.ToString()));
            CreateMap<Client, ExportClientDto>()
                .ForMember(dest => dest.ClientName, opt =>
                    opt.MapFrom(s => s.Name))
                .ForMember(dest => dest.VatNumber, opt =>
                    opt.MapFrom(s => s.NumberVat))
                .ForMember(dest => dest.Invoices, opt =>
                    opt.MapFrom(s => s.Invoices
                                        .ToArray()
                                        .OrderBy(i => i.IssueDate)
                                        .ThenByDescending(i => i.DueDate)));
        }
    }
}
