# SecondApp — öyrənmə layihəsi

Bu layihə ASP.NET Core öyrənmək üçündür. Hər commit ayrıca bir dərsdir.
Növbəti commit-də əvvəlki kod tamamilə silinib yeni mövzuya başlanıla bilər,
ona görə kodun izahı kodun özündə yox, **commit mesajında** saxlanılır.

## Commit qaydası

- Başlıq (1-ci sətir): qısa, mövzunun adı. Məsələn: `Content negotiation: server-driven və agent-driven`.
- Body: qısa təsvir **yox**, ətraflı **izah** yazılır, Azərbaycan dilində. Sonra yalnız commit mesajını
  oxuyaraq mövzunu yenidən başa düşmək mümkün olmalıdır. Body-də bunlar olsun:
  - Mövzu/konsept nədir və hansı problemi həll edir
  - Kod necə işləyir (əsas class, method, header-lər, status kodları)
  - Müqayisə varsa, fərqlər (qısa cədvəl)
  - Necə işə salmaq və yoxlamaq (`dotnet run`, `curl` əmrləri, gözlənilən nəticə)
  - Diqqət ediləsi məqamlar, tipik səhvlər
- Kod sonradan silinsə belə, izah ona istinad etmədən başa düşülən olmalıdır.
- Commit mesajına və PR təsvirinə heç vaxt `Co-Authored-By: Claude ...` və ya başqa Claude/AI attribution sətri yazılmır.

## Köhnə dərsə qayıtmaq

- `git log` — bütün dərslərin siyahısı
- `git show <hash>` — dərsin izahı və kodu
- `git checkout <hash>` — həmin dərsin kodunu işə salmaq üçün (sonra `git checkout master`)
