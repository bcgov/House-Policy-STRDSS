module "fdf5df_deployer" {
  source  = "bcgov/openshift/deployer"
  version = "0.11.0"

  name                  = "oc-deployer"
  namespace             = "f4a30d-prod"
  privileged_namespaces = ["f4a30d-prod"]
}
