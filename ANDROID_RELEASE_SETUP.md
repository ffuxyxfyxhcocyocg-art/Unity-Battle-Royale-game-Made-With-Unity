# Battle Royale Mobile — configuração Android

Este repositório contém o projeto Unity original em `BattleRoyale/` (Unity **2018.1.0f2**).

## Build Android automatizado

O workflow `.github/workflows/android-apk.yml` compila um APK assinado:
- manualmente em **Actions → Build Android APK → Run workflow**; ou
- ao enviar uma tag `v*` (ex.: `v0.1.1`), publicando o APK em GitHub Releases.

### Secrets obrigatórios

Em **Settings → Secrets and variables → Actions**, cadastre:

- `UNITY_LICENSE`: arquivo de licença serializado do Unity em formato XML.
- `ANDROID_KEYSTORE_BASE64`: arquivo keystore de release convertido para Base64.
- `ANDROID_KEYSTORE_PASS`: senha do keystore.
- `ANDROID_KEYALIAS_NAME`: alias da chave.
- `ANDROID_KEYALIAS_PASS`: senha do alias.

A keystore deve ser criada uma única vez e guardada em local seguro. **Nunca gere outra keystore para uma versão de atualização**: o Android exige a mesma chave para atualizar o APK já instalado. Não envie a keystore nem suas senhas ao Git.

### Identidade do app

O build define o application ID como `com.ffuxyxfyxhcocyocg.battleroyale`. Esse ID deve permanecer igual em todas as versões que atualizem a instalação existente.

O número de versão de build é incrementado pelo número da execução do GitHub Actions. O projeto ainda precisa ser aberto e validado no Unity antes de considerar a primeira compilação pronta; cenas ausentes, APIs antigas ou dependências podem exigir correções.

## Como funciona a atualização no Android

O workflow gera e publica o APK em Releases. **Isso, sozinho, não atualiza um jogo já instalado.** O jogo ainda precisa receber um componente de atualização dentro da aplicação, consultar a API de Releases e solicitar a instalação do APK novo.

Em Android comum, um app instalado por APK não pode substituir a si próprio silenciosamente: o instalador do Android exige confirmação do usuário. Atualização realmente automática e sem essa confirmação normalmente depende de distribuição pela Google Play (Play In-App Updates) ou de gerenciamento corporativo do dispositivo.

O atualizador dentro do jogo ainda não foi integrado; primeiro precisamos compilar e instalar uma primeira versão assinada para validar o ID do pacote e a chave.

## Atenção ao projeto original

A base usa Unity 2018.1.0f2 e APIs antigas de multiplayer. A automação de build não migra automaticamente a rede antiga nem garante que o multiplayer funcione em Android moderno.
