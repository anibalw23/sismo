
/*
Verifica si el Ciclo o el Nivel = 0
*/
SELECT Persona.Cedula as cedula, Persona.Nombres as Nombres, DocenteMateria.Nivel, DocenteMateria.Ciclo, 
DocenteMateria.Area, DocenteMateria.Grados, DocenteMateria.SeccionId, Personal.Id, DocenteMateria.DocenteId, Persona.Id, Personal.PersonaId
FROM DocenteMateria
INNER JOIN Personal
ON Personal.Id = DocenteMateria.DocenteId
INNER JOIN Persona
ON Persona.Id = Personal.PersonaId
where Ciclo = 0 or Nivel = 0



/*Revisar si el sexo esta vacio */

/*
Verifica si la n-upla Nivel, Ciclo, Grado, SeccionId, tanda esta repetido
*/
SELECT
    Nivel, Tanda, Ciclo, Area, Grados, SeccionId, DocenteId, COUNT(*) as theCount
FROM
    DocenteMateria
GROUP BY
   Nivel, Tanda, Ciclo, Area, Grados, SeccionId,DocenteId
HAVING 
    COUNT(*) > 1


WITH CTE AS
(
    SELECT  *,
            RN = ROW_NUMBER() OVER( PARTITION BY  Nivel, Tanda, Ciclo, Area, Grados, SeccionId, DocenteId
                                    ORDER BY DocenteId DESC)
    FROM DocenteMateria
)
Select * FROM CTE
WHERE RN > 1 





/************************************* Actividades de Acompanamiento ***********************************************************/


/* Verifica que las horas de acompañamiento no sean negativas */
select * from InscripcionActividadAcompanamiento where (horas < 0)
UPDATE InscripcionActividadAcompanamiento
SET horas = ABS(horas)
where (horas < 0) 
/* End Verifica horas de acompañamiento*/



/*
Verifica que las respuestas de las evaluaciones de las actividades de acompanamiento no se repiten 
*/
SELECT
    inscripcionActividadAcompanamientoId, preguntaAcompId, COUNT(*) as theCount
FROM
    EvaluacionAcompanamientoRespuesta
GROUP BY
   inscripcionActividadAcompanamientoId, preguntaAcompId
HAVING 
    COUNT(*) > 1


WITH CTE AS
(
    SELECT  *,
            RN = ROW_NUMBER() OVER( PARTITION BY inscripcionActividadAcompanamientoId, preguntaAcompId
                                    ORDER BY inscripcionActividadAcompanamientoId DESC)
    FROM EvaluacionAcompanamientoRespuesta
)
select * FROM CTE
WHERE RN > 1 



/*Verifica si la seccion es nula*/
select * from DocenteMateria where SeccionId is NULL