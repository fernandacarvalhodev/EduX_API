﻿using EduX_API.Controllers;
using EduX_API.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EduX_API.Interfaces
{
    interface IObjetivoAlunoRepository
    {
        List<ObjetivoAluno> Listar();
        ObjetivoAluno BuscarPorId(Guid Id);
        void Adicionar(ObjetivoAluno objetivoaluno);
        void Editar(ObjetivoAluno objetivoaluno);
        void Remover(Guid Id);
        List<ObjetivoAluno> ListarObjetivoPorAluno(Guid IdAlunoTurma, bool isPendente);
        object Ranking();
    }
}
