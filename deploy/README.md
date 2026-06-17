# GitHub 및 Oracle Cloud Ubuntu 배포 가이드

이 문서는 MoneyCalc.Web을 배포하면서 사용한 절차를 기준으로 정리한 재사용용
배포 체크리스트입니다. 다음 ASP.NET Core MVC 프로젝트에서도 프로젝트명, 도메인,
서버 IP, 서비스명만 바꾸면 거의 그대로 사용할 수 있습니다.

## 0. 현재 프로젝트 기준값

| 항목 | 값 |
| --- | --- |
| GitHub 저장소 | `https://github.com/oyj5291/MoneyCalc.Web` |
| 기본 브랜치 | `main` |
| 도메인 | `moneycalc.ai.kr` |
| 서버 IP | `168.107.22.55` |
| OS | Ubuntu 24.04 LTS |
| 앱 이름 | `moneycalc` |
| systemd 서비스 | `moneycalc.service` |
| 배포 경로 | `/var/www/moneycalc` |
| ASP.NET 포트 | `127.0.0.1:5000` |
| Nginx 포트 | `80`, `443` |

다른 프로젝트에서는 아래 값을 먼저 정해 둡니다.

```text
PROJECT_NAME=MyProject.Web
APP_NAME=myproject
DOMAIN=example.com
SERVER_IP=123.123.123.123
REMOTE_USER=ubuntu
DEPLOY_PATH=/var/www/myproject
APP_PORT=5000
```

## 1. 로컬 Git 준비

프로젝트 루트에서 현재 상태를 확인합니다.

```powershell
git status
```

처음 Git을 시작하는 프로젝트라면:

```powershell
git init
git add .
git commit -m "initial commit"
git branch -M main
git remote add origin https://github.com/사용자명/저장소명.git
git push -u origin main
```

이미 저장소가 연결된 프로젝트라면:

```powershell
git status
git add .
git commit -m "변경 내용 요약"
git push origin main
```

GitHub에서 확인할 것:

```text
1. 저장소가 Public 또는 서버에서 접근 가능한 Private인지
2. 기본 브랜치가 main인지
3. README, .gitignore, 소스 파일이 정상 업로드되었는지
```

## 2. Oracle Cloud 서버 준비

Oracle Cloud에서 Ubuntu 24.04 VM을 생성합니다.

권장 최소 사양:

```text
OS: Ubuntu 24.04 LTS
CPU: 1 Core 이상
Memory: 1GB 이상
Boot Volume: 40GB 이상
```

보안 목록 또는 NSG Ingress Rule:

| 포트 | 프로토콜 | 소스 | 용도 |
| --- | --- | --- | --- |
| 22 | TCP | `0.0.0.0/0` 또는 내 IP | SSH |
| 80 | TCP | `0.0.0.0/0` | HTTP |
| 443 | TCP | `0.0.0.0/0` | HTTPS |

PostgreSQL을 서버 내부에서만 사용할 경우 `5432`는 외부에 열지 않습니다.

Windows PowerShell에서 접속합니다.

```powershell
ssh -i C:\path\oracle.key ubuntu@SERVER_IP
```

최초 접속 후 서버 업데이트:

```bash
sudo apt update
sudo apt upgrade -y
sudo reboot
```

재부팅 후 다시 접속합니다.

## 3. .NET Runtime 설치

Microsoft 패키지 저장소를 등록합니다.

```bash
wget https://packages.microsoft.com/config/ubuntu/24.04/packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt update
```

서버에서 실행만 할 경우 Runtime만 설치합니다.

```bash
sudo apt install -y aspnetcore-runtime-10.0
dotnet --list-runtimes
```

서버에서 직접 `dotnet publish`까지 할 경우 SDK도 설치합니다.

```bash
sudo apt install -y dotnet-sdk-10.0
dotnet --info
```

## 4. Git, Nginx, Certbot 설치

```bash
sudo apt install -y git nginx certbot python3-certbot-nginx
```

Ubuntu 방화벽을 사용할 경우:

```bash
sudo ufw allow OpenSSH
sudo ufw allow 'Nginx Full'
sudo ufw enable
sudo ufw status
```

주의: Oracle Cloud 보안 목록과 Ubuntu UFW가 모두 열려 있어야 외부 접속이 됩니다.
브라우저에서 타임아웃이 나면 Oracle Cloud 보안 목록을 먼저 확인합니다.

## 5. 로컬에서 게시 파일 생성

솔루션 루트에서 빌드와 게시를 실행합니다.

```powershell
dotnet build
dotnet publish .\MoneyCalc.Web\MoneyCalc.Web.csproj -c Release -o .\artifacts\publish
```

프로젝트명이 다르면 `.csproj` 경로만 바꿉니다.

게시 결과:

```text
.\artifacts\publish
```

## 6. 게시 파일 서버 전송

서버 임시 폴더를 먼저 만듭니다.

```powershell
ssh -i C:\path\oracle.key ubuntu@SERVER_IP "mkdir -p /tmp/moneycalc"
```

게시 파일과 설정 파일을 전송합니다.

```powershell
scp -i C:\path\oracle.key -r .\artifacts\publish\* ubuntu@SERVER_IP:/tmp/moneycalc/
scp -i C:\path\oracle.key .\deploy\moneycalc.service ubuntu@SERVER_IP:/tmp/
scp -i C:\path\oracle.key .\deploy\nginx-moneycalc.conf ubuntu@SERVER_IP:/tmp/
```

다음 프로젝트에서는 `moneycalc` 부분을 앱 이름으로 변경합니다.

## 7. systemd 서비스 등록

서버에서 앱 폴더를 만들고 게시 파일을 복사합니다.

```bash
sudo mkdir -p /var/www/moneycalc
sudo cp -a /tmp/moneycalc/. /var/www/moneycalc/
sudo chown -R www-data:www-data /var/www/moneycalc
sudo chmod -R u=rwX,g=rX,o= /var/www/moneycalc
```

서비스 파일을 등록합니다.

```bash
sudo cp /tmp/moneycalc.service /etc/systemd/system/moneycalc.service
sudo systemctl daemon-reload
sudo systemctl enable --now moneycalc
sudo systemctl status moneycalc --no-pager
```

서비스 파일 예시:

```ini
[Unit]
Description=MoneyCalc Web
After=network.target

[Service]
WorkingDirectory=/var/www/moneycalc
ExecStart=/usr/bin/dotnet /var/www/moneycalc/MoneyCalc.Web.dll
Restart=always
RestartSec=10
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://127.0.0.1:5000

[Install]
WantedBy=multi-user.target
```

로컬 앱 확인:

```bash
curl --fail http://127.0.0.1:5000/healthz
```

로그 확인:

```bash
sudo journalctl -u moneycalc -n 100 --no-pager
sudo journalctl -u moneycalc -f
```

## 8. 환경 변수와 비밀값 관리

DB 비밀번호, API 키 같은 값은 Git에 올리지 않습니다. systemd drop-in 파일로
관리합니다.

```bash
sudo mkdir -p /etc/systemd/system/moneycalc.service.d
sudo nano /etc/systemd/system/moneycalc.service.d/env.conf
```

예시:

```ini
[Service]
Environment=ConnectionStrings__Default=Host=localhost;Port=5432;Database=moneycalc;Username=moneycalc_app;Password=강한비밀번호
```

적용:

```bash
sudo systemctl daemon-reload
sudo systemctl restart moneycalc
sudo systemctl status moneycalc --no-pager
```

환경 변수 확인이 필요할 때:

```bash
sudo systemctl show moneycalc --property=Environment
```

## 9. Nginx 리버스 프록시 설정

Nginx 설정 파일을 등록합니다.

```bash
sudo cp /tmp/nginx-moneycalc.conf /etc/nginx/sites-available/moneycalc
sudo ln -s /etc/nginx/sites-available/moneycalc /etc/nginx/sites-enabled/moneycalc
sudo rm -f /etc/nginx/sites-enabled/default
sudo nginx -t
sudo systemctl reload nginx
```

Nginx 설정 예시:

```nginx
server {
    listen 80;
    server_name moneycalc.ai.kr www.moneycalc.ai.kr;

    location / {
        proxy_pass http://127.0.0.1:5000;
        proxy_http_version 1.1;

        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

HTTP 접속 확인:

```bash
curl -I http://moneycalc.ai.kr
```

## 10. DNS 연결

도메인 관리 사이트에서 A 레코드를 등록합니다.

| 이름 | 타입 | 값 |
| --- | --- | --- |
| `@` | A | `SERVER_IP` |
| `www` | A | `SERVER_IP` |

DNS 반영 확인:

```powershell
nslookup moneycalc.ai.kr
nslookup www.moneycalc.ai.kr
```

브라우저에서 `http://도메인` 접속이 되어야 HTTPS 발급을 진행할 수 있습니다.

## 11. HTTPS 적용

DNS가 서버 IP로 정상 연결되고 HTTP 접속이 확인된 뒤 실행합니다.

```bash
sudo certbot --nginx -d moneycalc.ai.kr -d www.moneycalc.ai.kr
```

인증서 자동 갱신 테스트:

```bash
sudo certbot renew --dry-run
```

최종 확인:

```bash
curl --fail https://moneycalc.ai.kr/healthz
curl --fail https://moneycalc.ai.kr/sitemap.xml
curl --fail https://moneycalc.ai.kr/robots.txt
```

## 12. PostgreSQL 16 선택 설치

게시판, 회원, 로그 저장 등 DB가 필요한 경우에만 설치합니다.

```bash
sudo apt install -y postgresql-16 postgresql-client-16
sudo systemctl enable --now postgresql
sudo systemctl status postgresql --no-pager
```

DB와 사용자 생성 예시:

```bash
sudo -u postgres createuser moneycalc_app
sudo -u postgres psql -c "ALTER ROLE moneycalc_app WITH LOGIN PASSWORD '강한비밀번호';"
sudo -u postgres createdb -O moneycalc_app moneycalc
sudo -u postgres psql -d moneycalc -c "ALTER SCHEMA public OWNER TO moneycalc_app;"
sudo -u postgres psql -d moneycalc -c "GRANT ALL ON SCHEMA public TO moneycalc_app;"
```

연결 문자열은 `/etc/systemd/system/moneycalc.service.d/env.conf`에 넣습니다.
PostgreSQL 포트 `5432`는 외부에 공개하지 않는 것을 기본값으로 둡니다.

## 13. 이후 버전 업데이트

로컬에서 다시 게시합니다.

```powershell
dotnet publish .\MoneyCalc.Web\MoneyCalc.Web.csproj -c Release -o .\artifacts\publish
```

서버로 전송합니다.

```powershell
ssh -i C:\path\oracle.key ubuntu@SERVER_IP "rm -rf /tmp/moneycalc && mkdir -p /tmp/moneycalc"
scp -i C:\path\oracle.key -r .\artifacts\publish\* ubuntu@SERVER_IP:/tmp/moneycalc/
```

서버에서 교체합니다.

```bash
sudo systemctl stop moneycalc
sudo cp -a /tmp/moneycalc/. /var/www/moneycalc/
sudo chown -R www-data:www-data /var/www/moneycalc
sudo systemctl start moneycalc
sudo systemctl status moneycalc --no-pager
curl --fail http://127.0.0.1:5000/healthz
```

정상 확인 후 브라우저에서:

```text
https://moneycalc.ai.kr
```

## 14. 장애 확인 순서

브라우저에서 타임아웃:

```text
1. Oracle Cloud 보안 목록에서 80, 443이 열려 있는지 확인
2. Ubuntu UFW에서 Nginx Full이 허용되어 있는지 확인
3. Nginx가 실행 중인지 확인
```

확인 명령:

```bash
sudo ufw status
sudo systemctl status nginx --no-pager
sudo ss -tlnp
```

502 Bad Gateway:

```text
Nginx는 살아 있지만 ASP.NET 앱이 죽었거나 포트가 다릅니다.
```

확인 명령:

```bash
sudo systemctl status moneycalc --no-pager
sudo journalctl -u moneycalc -n 100 --no-pager
curl -I http://127.0.0.1:5000
```

HTTPS 인증서 실패:

```text
1. 도메인 A 레코드가 서버 IP를 가리키는지 확인
2. HTTP 접속이 먼저 되는지 확인
3. Nginx server_name이 도메인과 일치하는지 확인
```

확인 명령:

```bash
sudo nginx -t
sudo certbot certificates
sudo tail -n 100 /var/log/nginx/error.log
```

DB 연결 실패:

```bash
sudo systemctl show moneycalc --property=Environment
sudo systemctl status postgresql --no-pager
sudo -u postgres psql -d moneycalc -c "\dt"
```

## 15. 배포 후 SEO 확인

배포가 끝나면 아래 URL을 브라우저에서 확인합니다.

```text
https://도메인/
https://도메인/robots.txt
https://도메인/sitemap.xml
https://도메인/healthz
```

Google Search Console:

```text
1. 도메인 또는 URL 접두어 속성 등록
2. sitemap.xml 제출
3. 주요 페이지 URL 검사
4. 색인 생성 요청
```

## 16. 다음 프로젝트용 빠른 체크리스트

```text
[ ] GitHub 저장소 생성
[ ] 로컬 Git commit / push
[ ] Oracle Cloud Ubuntu VM 생성
[ ] Oracle 보안 목록 22, 80, 443 개방
[ ] SSH 접속 확인
[ ] 서버 apt update / upgrade
[ ] .NET Runtime 설치
[ ] Nginx / Certbot 설치
[ ] dotnet publish
[ ] scp로 게시 파일 전송
[ ] /var/www/앱이름 복사
[ ] systemd 서비스 등록
[ ] curl localhost 확인
[ ] Nginx reverse proxy 설정
[ ] DNS A 레코드 연결
[ ] HTTP 접속 확인
[ ] Certbot HTTPS 적용
[ ] HTTPS 접속 확인
[ ] robots.txt / sitemap.xml 확인
[ ] Search Console 등록
```
