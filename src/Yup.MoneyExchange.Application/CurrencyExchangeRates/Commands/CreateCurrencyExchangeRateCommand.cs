﻿using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Yup.MoneyExchange.Domain.Repositories;
using Yup.MoneyExchange.Domain.AggregatesModel;
using Yup.MoneyExchange.Application.Dtos;

namespace Yup.MoneyExchange.Application.CurrencyExchangeRates.Commands;

public class CreateCurrencyExchangeRateCommand : IRequest<GenericResult>
{
    /// <summary>
    /// Identificador de la moneda de origen
    /// </summary>
    public Guid CurrencyFromId { get; set; }
    /// <summary>
    /// Identificador de la moneda de destino
    /// </summary>
    public Guid CurrencyToId { get; set; }
    /// <summary>
    /// Tipo de cambio de la moneda origen a la moneda de destino
    /// </summary>
    public decimal Exchange { get; set; }
    public decimal? PreferencialExchange { get; set; }
    [JsonIgnore]
    public Guid RegistredBy { get; set; }
    public CreateCurrencyExchangeRateCommand(Guid currencyFromId, Guid currencyToId, decimal exchange, decimal? preferencialExchange, Guid registredBy)
    {
        CurrencyFromId = currencyFromId;
        CurrencyToId = currencyToId;
        Exchange = exchange;
        PreferencialExchange = preferencialExchange;
        RegistredBy = registredBy;
    }

    public class CreateCurrencyExchangeRateCommandHandler : IRequestHandler<CreateCurrencyExchangeRateCommand, GenericResult>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseRepository<CurrencyExchangeRate> _currencyExchangeRateRepository;

        public CreateCurrencyExchangeRateCommandHandler(IUnitOfWork unitOfWork, IBaseRepository<CurrencyExchangeRate> currencyExchangeRateRepository)
        {
            _unitOfWork = unitOfWork;
            _currencyExchangeRateRepository = currencyExchangeRateRepository;
        }

        public async Task<GenericResult> Handle(CreateCurrencyExchangeRateCommand request, CancellationToken cancellationToken)
        {
            var result = new GenericResult();
            //Validaciones
            //TODO: Llevar a FluentValidation

            var currencyExchangeRateToSave = new CurrencyExchangeRate(request.CurrencyFromId, request.CurrencyToId, request.Exchange, request.PreferencialExchange);

            currencyExchangeRateToSave.SetCreateAudit(DateTime.Now, request.RegistredBy);
            var resultAdd = _currencyExchangeRateRepository.Add(currencyExchangeRateToSave);

            var saved = await _unitOfWork.SaveChangesAsync();

            return result;
        }
    }
}
