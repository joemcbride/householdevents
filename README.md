# Household Events

## Deploying to Heroku

Add the following build packs

* heroku buildpacks:set https://github.com/jincod/dotnetcore-buildpack
* heroku buildpacks:add --index 1 heroku/nodejs


### Connection String

To enable running on Heroku Postgres from your local machine you need to add a couple properties to your connection string.

* `sslmode=Require` - Heroku requires this for external connections [see here](https://devcenter.heroku.com/articles/heroku-postgresql#external-connections-ingress)
* `Trust Server Certificate=true` - Heroku servers use a self-signed certificate [see here](http://www.npgsql.org/doc/security.html#encryption-ssltls)

```
host=host.com;database=dbname;password=password;username=user;sslmode=Require;Trust Server Certificate=true
```
