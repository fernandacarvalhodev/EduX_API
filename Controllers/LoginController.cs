﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using EduX_API.Context;
using EduX_API.Domains;
using EduX_API.Utilis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace EduX_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        //Chamamos o contexto do banco
        EduXContext _context;

        // Definimos uma variável para percorrer nossos métodos com as configurações obtidas no appsettings.json
        private IConfiguration _config;

        // Definimos um método construtor para poder passar essas configs
        public LoginController(IConfiguration config)
        {
            _config = config;
            _context = new EduXContext();
        }

        private Usuario AuthenticateUser(Login login)
        {

            return _context.Usuario
                .Include(u => u.Perfil)
                .Where(u => u.Email == login.Email && u.Senha == login.Senha)
                .FirstOrDefault();
        }

        // Criamos nosso método que vai gerar nosso Token
        private string GenerateJSONWebToken(Usuario userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Definimos nossas Claims (dados da sessão) para poderem ser capturadas
            // a qualquer momento enquanto o Token for ativo
            var claims = new[] {
            new Claim(JwtRegisteredClaimNames.NameId, userInfo.Nome),
            new Claim(JwtRegisteredClaimNames.UniqueName, userInfo.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, userInfo.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, userInfo.IdPerfil.ToString()),
            new Claim("role", userInfo.IdPerfil.ToString()),
            new Claim("permissao", userInfo.Perfil.Permissao)
            };

            // Configuramos nosso Token e seu tempo de vida
            var token = new JwtSecurityToken
                (
                    _config["Jwt:Issuer"],
                    _config["Jwt:Issuer"],
                    claims,
                    expires: DateTime.Now.AddMinutes(120),
                    signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Usamos a anotação "AllowAnonymous" para 
        // ignorar a autenticação neste método, já que é ele quem fará isso
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] Login login)
        {
            // Definimos primeiro como não autorizado
            IActionResult response = Unauthorized();

            // Autenticamos o usuário da API
            var user = AuthenticateUser(login);
            if (user != null)
            {
                var tokenString = GenerateJSONWebToken(user);
                login.Senha = Crypto.Criptografar(login.Senha, login.Email.Substring(0, 5));
                response = Ok(new { token = tokenString });
            }

            return response;
        }
    }
}