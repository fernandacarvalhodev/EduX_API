﻿using EduX_API.Context;
using EduX_API.Domains;
using EduX_API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EduX_API.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    { 
        private readonly EduXContext _ctx;

        public UsuarioRepository()
        {
            _ctx = new EduXContext();
        }

        /// <summary>
        /// Adicionar um novo usuário
        /// </summary>
        /// <param name="usuario">Novo suário</param>
        public void Adicionar(Usuario usuario)
        {
            try
            { 
                //Adicionar aluno
                _ctx.Usuario.Add(usuario);
                //SalvarAlterações
                _ctx.SaveChanges();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Buscar o usuário pelo seu ID
        /// </summary>
        /// <param name="Id">Id do usuário a ser procurado</param>
        /// <returns>Usuário encontrado</returns>
        public Usuario BuscarPorId(Guid Id)
        {
            try
            {
                //Achar usuário por Id
                return _ctx.Usuario.Find(Id);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Buscar o usuário pelo seu Nome
        /// </summary>
        /// <param name="Nome">Nome do usuário a ser procurado</param>
        /// <returns>Usuário encontrado</returns>
        public List<Usuario> BuscarPorNome(string Nome)
        {
            try
            {
                List<Usuario> usuario = _ctx.Usuario.Where(c => c.Nome.Contains(Nome)).ToList();
                return usuario;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Fazer mudanças no usuário
        /// </summary>
        /// <param name="usuario">Usuário</param>
        public void Editar(Usuario usuario)
        {
            try
            {
                //Buscar por Id
                Usuario usuario1 = BuscarPorId(usuario.Id);

                //Verificar se o usuário existe (ou não)
                if (usuario1 == null)
                    throw new Exception("Usuário não encontrado");

                //Dados do usuário que podem ser alterados
                usuario1.Nome = usuario.Nome;
                usuario1.Senha = usuario.Senha;
                usuario1.Email = usuario.Email;

                //Alterar
                _ctx.Usuario.Update(usuario1);
                //Salvar
                _ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Listar os usuários cadastrados
        /// </summary>
        /// <returns>Lista de usuários cadastrados</returns>
        public List<Usuario> Listar()
        {
            try
            {
                //Retornar minha lista de usuários
                List<Usuario> usuario = _ctx.Usuario.Include(c => c.Perfil).ToList();
                return usuario;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Excluir um usuário cadastrado
        /// </summary>
        /// <param name="Id">Id do usuário excluído</param>
        public void Remover(Guid Id)
        {
            try
            {
                //Buscar o Id do Usuario
                Usuario usuario = BuscarPorId(Id);

                //Verificar se o usuario existe (ou não)
                if (usuario == null)
                    throw new Exception("Usuario não encontrado");

                //Remove o usuário
                _ctx.Usuario.Remove(usuario);
                //Salva alterações
                _ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
