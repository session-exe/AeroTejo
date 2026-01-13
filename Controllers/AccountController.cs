using Microsoft.AspNetCore.Mvc;
using AeroTejo.Data;
using AeroTejo.Models;
using AeroTejo.Helpers;
using AeroTejo.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace AeroTejo.Controllers
{
    /// <summary>
    /// Controller responsável pela autenticação e gestão de contas de utilizador
    /// </summary>
    public class AccountController : Controller
    {
        private readonly AeroTejoContext _context;

        public AccountController(AeroTejoContext context)
        {
            _context = context;
        }

        // GET: Account/Register
        /// <summary>
        /// Apresenta o formulário de registo de novo utilizador
        /// </summary>
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        /// <summary>
        /// Processa o registo de um novo utilizador
        /// O utilizador recebe automaticamente o papel de "Passageiro"
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Verificar se o e-mail já está registado
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email);

                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Este e-mail já está registado.");
                    return View(model);
                }

                // Gerar salt e hash da password
                string salt = PasswordHelper.GenerateSalt();
                string passwordHash = PasswordHelper.HashPassword(model.Password, salt);

                // Criar novo utilizador com papel de "Passageiro" automaticamente
                var user = new User
                {
                    NomeCompleto = model.NomeCompleto,
                    Idade = model.Idade,
                    Email = model.Email,
                    Telemovel = model.Telemovel,
                    PasswordHash = passwordHash,
                    PasswordSalt = salt,
                    Role = "Passageiro" // Atribuição automática do papel
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Registo efetuado com sucesso! Pode agora fazer login.";
                return RedirectToAction(nameof(Login));
            }

            return View(model);
        }

        // GET: Account/Login
        /// <summary>
        /// Apresenta o formulário de login
        /// </summary>
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        /// <summary>
        /// Processa o login do utilizador
        /// Verifica o e-mail e password usando hashing com salt
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Procurar utilizador pelo e-mail
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email);

                if (user != null)
                {
                    // Verificar a password usando o helper
                    bool isPasswordValid = PasswordHelper.VerifyPassword(
                        model.Password, 
                        user.PasswordHash, 
                        user.PasswordSalt
                    );

                    if (isPasswordValid)
                    {
                        // Guardar informações do utilizador na sessão
                        HttpContext.Session.SetInt32("UserId", user.Id);
                        HttpContext.Session.SetString("UserName", user.NomeCompleto);
                        HttpContext.Session.SetString("UserRole", user.Role);
                        HttpContext.Session.SetString("UserEmail", user.Email);

                        // Redirecionar conforme o papel do utilizador
                        if (user.Role == "Administrador")
                        {
                            return RedirectToAction("Dashboard", "Admin");
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }

                // Se chegou aqui, as credenciais são inválidas
                ModelState.AddModelError(string.Empty, "E-mail ou password inválidos.");
            }

            return View(model);
        }

        // GET: Account/Logout
        /// <summary>
        /// Termina a sessão do utilizador
        /// </summary>
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "Sessão terminada com sucesso.";
            return RedirectToAction("Index", "Home");
        }
    }
}
