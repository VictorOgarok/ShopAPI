FROM mcr.microsoft.com/dotnet/sdk:6.0 as build

ARG BUILDCONFIG=RELEASE
ARG VERSION=1.0.0

COPY ShopAPI.csproj /build/

RUN dotnet restore ./build/ShopAPI.csproj

COPY . ./build/
WORKDIR /build/
RUN dotnet publish ./ShopAPI.csproj -c $BUILDCONFIG -o out /p:Version=$VERSION

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app

COPY --from=build /build/out .

ENTRYPOINT ["dotnet", "ShopAPI.dll"]