using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorGrpcDemo.Shared;
using BlazorGrpcDemo.Server.Data;

public class PeopleService : People.PeopleBase
{
    PersonsManager personsManager;

    public PeopleService(PersonsManager _personsManager)
    {
        personsManager = _personsManager;
    }

    public override Task<PeopleReply> GetAll(GetAllPeopleRequest request,
        ServerCallContext context)
    {
        var reply = new PeopleReply();
        reply.People.AddRange(personsManager.People);
        return Task.FromResult(reply);
    }

    public override async Task GetAllStream(GetAllPeopleRequest request,
        IServerStreamWriter<Person> responseStream, ServerCallContext context)
    {
        // Use this pattern to return a stream in a gRPC service.

        // retrieve the list
        var people = personsManager.People;

        // write each item to the responseStream, which does the rest
        foreach (var person in people)
        {
            await responseStream.WriteAsync(person);
        }
    }

    public override Task<Person> GetPersonById(GetPersonByIdRequest request,
        ServerCallContext context)
    {
        var result = (from x in personsManager.People
                      where x.Id == request.Id
                      select x).FirstOrDefault();
        return Task.FromResult(result);
    }
}
