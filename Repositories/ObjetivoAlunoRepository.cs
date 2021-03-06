﻿using EduX_API.Context;
using EduX_API.Domains;
using EduX_API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace EduX_API.Repositories
{
    public class ObjetivoAlunoRepository : IObjetivoAlunoRepository
    {
        //Trazemos nosso contexto
        private readonly EduXContext _ctx;

        public ObjetivoAlunoRepository()
        {
            _ctx = new EduXContext();
        }

        /// <summary>
        /// Adicionar um novo ObjetivoAluno
        /// </summary>
        /// <param name="objetivoaluno">Novo ObjetivoAluno</param>
        public void Adicionar(ObjetivoAluno objetivoaluno)
        {
            try
            {  
                //Adiciona um ObjetivoAluno
                _ctx.ObjetivoAluno.Add(objetivoaluno);
                //Salvar mudanças
                _ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Buscar ObjetivoAluno por Id
        /// </summary>
        /// <param name="Id">Id procurado</param>
        /// <returns>O ObjetivoAluno encontrado</returns>
        public ObjetivoAluno BuscarPorId(Guid Id)
        {
            try
            {
                //Achar o objetivo aluno pelo Id
                return _ctx.ObjetivoAluno.Find(Id);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Alterar ObjetivoAluno
        /// </summary>
        /// <param name="objetivoaluno">ObjetivoAluno alterado</param>
        public void Editar(ObjetivoAluno objetivoaluno)
        {
            try
            {
                //Buscar objetivo aluno pelo Id
                ObjetivoAluno objetivoAlunoLoaded = BuscarPorId(objetivoaluno.Id);

                //Verifica se existe (ou não)
                if (objetivoAlunoLoaded == null)
                    throw new Exception("ObjetivoAluno não encontrado");

                //Dados que podem ser alterados
                if (objetivoaluno.Nota != null)
                {
                    objetivoAlunoLoaded.Nota = objetivoaluno.Nota;
                    objetivoAlunoLoaded.DataAlcancado = DateTime.Now;
                }
                objetivoAlunoLoaded.UrlImagem = objetivoaluno.SalvarArquivo(objetivoaluno);

                //Alterar
                _ctx.ObjetivoAluno.Update(objetivoAlunoLoaded);

                //Salvar
                _ctx.SaveChanges();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Listar os ObjetivosAluno
        /// </summary>
        /// <returns>Lista de objetivos</returns>
        public List<ObjetivoAluno> Listar()
        {
            try
            {
                //Retornar minha lista de ObjetivoAluno
                return _ctx.ObjetivoAluno.Include(c => c.AlunoTurma.Usuario).Include(c => c.Objetivo).ToList();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<ObjetivoAluno> ListarObjetivoPorAluno(Guid IdAlunoTurma, bool isPendente)
        {
            try
            {
                if (isPendente)
                {
                    return _ctx.ObjetivoAluno
                      .Include(obj => obj.AlunoTurma)
                      .Include(obj => obj.Objetivo).ThenInclude(obj => obj.Categoria)
                      .Where(obj => obj.IdAlunoTurma.Equals(IdAlunoTurma) && string.IsNullOrEmpty(obj.UrlImagem)).ToList();
                }
                return _ctx.ObjetivoAluno
                       .Include(obj => obj.AlunoTurma)
                       .Include(obj => obj.Objetivo).ThenInclude(obj => obj.Categoria)
                       .Where(obj => obj.IdAlunoTurma.Equals(IdAlunoTurma) && !string.IsNullOrEmpty(obj.UrlImagem)).ToList();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Remover um ObjetivoAluno
        /// </summary>
        /// <param name="Id">Id do ObjetivoAluno excluído</param>
        public void Remover(Guid Id)
        {
            try {
                //Busca o Id do objetivo aluno
                ObjetivoAluno objetivoaluno1 = BuscarPorId(Id);

                //Verifica se o objetivoAluno existe (ou não)
                if (objetivoaluno1 == null)
                    throw new Exception("Objetivo Aluno não encontrado");

                //Remover objetivo aluno
                _ctx.ObjetivoAluno.Remove(objetivoaluno1);
                //Salva alterações
                _ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            } 
        }
        public object Ranking()
        {
            try
            {
                var queryAlunoTurma = (from al in _ctx.AlunoTurma
                                       join user in _ctx.Usuario on al.IdUsuario equals user.Id
                                       select new
                                       {
                                           idusuario = user.Id,
                                           idAlunoTurma = al.Id,
                                           nome = user.Nome
                                       }).ToList();

                var queryRanking = (from obja in _ctx.ObjetivoAluno
                                    group new { obja.Id, obja.IdAlunoTurma, obja.Nota } by obja.IdAlunoTurma into g
                                    select new
                                    {
                                        idAlunoTuma = g.Key,
                                        media = g.Average(o => o.Nota).HasValue ? g.Average(o => o.Nota) : 0
                                    } into r orderby r.media descending
                                      select new
                                      {
                                          idAlunoTuma = r.idAlunoTuma,
                                          media = r.media
                                      }
                                      ).Take(3).ToList();
                List<object> results = new List<object>();
                foreach (var item in queryRanking)
                {
                    var al = queryAlunoTurma.Find(al => al.idAlunoTurma.Equals(item.idAlunoTuma));
                    results.Add(new {
                        idUsuario = al.idusuario,
                        idAlunoTurma = al.idAlunoTurma,
                        nome = al.nome,
                        media = item.media
                    });
                }

                return results;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
