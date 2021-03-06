﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using EduX_API.Domains;
using EduX_API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EduX_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstituicaoController : ControllerBase
    {
        private readonly InstituicaoRepository _instituicaoRepository;
        public InstituicaoController()
        {
            _instituicaoRepository = new InstituicaoRepository();
        }
        //listar
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var instituicoes = _instituicaoRepository.Listar();
                if (instituicoes.Count == 0)
                {
                    return NoContent();
                }
                return Ok(new
                {
                    totalCount = instituicoes.Count,
                    data = instituicoes
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    error = ex.Message
                });
            }
        }
        //Buscar por ID
        [HttpGet("{Id}")]
        public IActionResult Get(Guid Id)
        {
            try
            {
                Instituicao instituicao = _instituicaoRepository.BuscarPorId(Id);
                if (instituicao == null)
                {
                    return NoContent();
                }
                return Ok(new
                {
                    data = instituicao
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    error = ex.Message
                });
            }
        }
        //Buscar por nome :)
        [HttpGet("GetByNome/{Id}")]
        
        public IActionResult GetByNome(Guid Id)
        {
            try
            {
                List<Instituicao> instituicao = _instituicaoRepository.BuscarPorNome("Nome");
                if (instituicao == null)
                {
                    return NoContent();
                }
                return Ok(new
                {
                    data = instituicao
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    error = ex.Message
                });
            }
        }
        [HttpPut]
        public IActionResult Put([FromBody] Instituicao instituicao)
        {
            try
            {
                _instituicaoRepository.Adicionar(instituicao);
                return Ok(instituicao);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("{Id}")]
        public IActionResult Delete(Guid Id)
        {
            try
            {
                _instituicaoRepository.Remover(Id);
                return Ok(Id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("{Id}")]
        public IActionResult Update(Guid Id, Instituicao instituicao)
        {
            try
            {
                var instituicaoContexto = _instituicaoRepository.BuscarPorId(Id);
                if (instituicaoContexto == null)
                {
                    return NotFound();
                }
                instituicao.Id = Id;
                _instituicaoRepository.Editar(instituicao);
                return Ok(instituicao);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public IActionResult Post([FromBody] Instituicao instituicao)
        {
            try
            {
                //Adicionar uma instituição
                _instituicaoRepository.Adicionar(instituicao);

                //Retorna OK e as instituições
                return Ok(instituicao);
            }
            catch (Exception ex)
            {
                //Caso ocorra um erro retorna uma mensagem de erro
                return BadRequest(ex.Message);
            }
        }
    }
}