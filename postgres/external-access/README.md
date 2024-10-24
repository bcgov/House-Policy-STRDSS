# Allowing DB access for Power BI reports/dashboards

Refer to [How to expose non-HTTP access to a service](https://digital.gov.bc.ca/cloud/services/private/internal-resources/emerald/#how-do-i-expose-nonhttp-access-to-a-service)

## Create a service with LoadBalancer type

Run the command on the namespace of the environment.

```sh
oc apply -f strdss<env>-lb.yaml
```

## Get assigned IP address

```sh
oc -n b0471a-<env> get service strdss<env>-lb -o jsonpath='{.status.loadBalancer.ingress[].ip}{"\n"}'
```

## Creating Read Only User

After replacing `<username>` and `<dbname>`, run the [create-read-only-user script](readonly-user.sql) to create a read-only user.

## Dropping user

If you want to delete the user, after replacing `<username>` and `<dbname>`, run the [drop-user script](drop-user.sql).