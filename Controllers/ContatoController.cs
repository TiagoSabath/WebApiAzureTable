using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;
using WebApiAzureTable.Models;

namespace WebApiAzureTable.Controllers
{
    
    [ApiController]
    [Route("[controller]")]
    public class ContatoController : ControllerBase
    {
        private readonly string _conectionString;
        private readonly string _tableName;

        public ContatoController(IConfiguration configuration)
        {
            _conectionString = configuration.GetValue<string>("SAConnectionString");
            _tableName = configuration.GetValue<string>("AzureTableName");
        }

        private TableClient GetTableClient()
        {
            var serviceCLient = new TableServiceClient(_conectionString);
            var tableClient = serviceCLient.GetTableClient(_tableName);

            tableClient.CreateIfNotExists();
            return tableClient;
        }


        [HttpPost("Inserir Contato")]
        public IActionResult CriarContato(Contato contato)
        {
            var tableClient = GetTableClient();

            contato.RowKey = Guid.NewGuid().ToString();
            contato.PartitionKey = contato.RowKey;

            tableClient.UpsertEntity(contato);

            return Ok(contato);
        }

        [HttpPut("Atualizar Contato{id}")]
        public IActionResult Atualizar(string id, Contato contato)
        {
            var tableClient = GetTableClient();
            var contatoTable = tableClient.GetEntity<Contato>(id, id).Value;

            contatoTable.Nome = contato.Nome;
            contatoTable.Email = contato.Email;
            contatoTable.Telefone = contato.Telefone;

            tableClient.UpsertEntity(contatoTable);
            return Ok();
        }

        [HttpGet("Listar Contato")]
        public IActionResult ObterTodos()
        {
            var tableClient = GetTableClient();
            var contato = tableClient.Query<Contato>().ToList();
            return Ok(contato);
        }

        [HttpGet("Obter Contato/{nome}")]
        public IActionResult ObterNome(string nome)
        {
            var tableClient = GetTableClient();
            var contatos = tableClient.Query<Contato>(x => x.Nome == nome).ToList();
            return Ok(contatos);
        }

        [HttpDelete("{id}")]
        public IActionResult Deletar(string id)
        {
            var tableClient = GetTableClient();
            tableClient.DeleteEntity(id, id); 
            return NoContent();
        }
    }
}