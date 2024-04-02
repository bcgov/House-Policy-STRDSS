# House-Policy-STRDSS
Short Term Rental Data Sharing System (STRDSS)

## Create Release Tag Procedure

The procedure outlines the steps to initiate a release and manage version. Here's a breakdown of each step:

### Procedure

To initiate a release, please follow these steps:

1. Ensure that the necessary changes intended for the release in the `main` environment are made and committed to the `main` branch.
1. Go to a manual GitHub Action [Create Version Tag with Changelog PR](./.github/workflows/create-tag-changelog-pr.yml).
1. Click `Run workflow` button and Select branch `main`.
1. Enter the tag version to create, without any prefixes, e.g. 1.2.3
1. Click `Run workflow` green button to trigger the process.
1. After the workflow runs successfully, ensure that the [deployment pipeline](./.github/workflows/deploy-test.yml) associated with the `main` environment is automatically triggered and completes.

## Continuous Integration and Deployment (CI/CD) Pipelines Overview

Our CI/CD process is facilitated through GitHub Actions, ensuring seamless integration of code into the repository and efficient deployment to the intended environments. Here's a breakdown of the pipelines in place:

1. [Deploy Dev Environment](./.github/workflows/deploy-dev.yml):

   - Generates new images based on commit hashes for deployment in the `Development` environment.
   - Triggered automatically upon new changes pushed to the `main` branch.

1. [Deploy Test Environment](./.github/workflows/deploy-test.yml):

   - Generates new images using tags for deploying changes to the `Testing` environment.
   - Triggered upon creation of a new tag prefixed with 'v'.

1. [Deploy UAT Environment](./.github/workflows/deploy-uat.yml):

   - Facilitates deployment of selected tag's images into the `UAT` environment via the GitHub UI.
   - Triggered through the GitHub UI, allowing for tag version selection.

1. [Deploy Prod Environment](./.github/workflows/deploy-prod.yml):

   - Facilitates deployment of selected tag's images into the `Production` environment.
   - Triggered through the GitHub UI upon publishing a new release from a tag.

1. [Create Version Tag with Changelog PR](./.github/workflows/create-tag-changelog-pr.yml):

   - Supports for generating version tags.
   - Triggered through the GitHub UI, allowing specification of tag versions excluding prefixes.

## Hotfix Procedure

When it's necessary to deploy a hotfix (critical fix) to Production, the steps below identify how to deploy the hotfix to production and then update the main branch with the hotfix code.

### Procedure
1. Identify the tag where the hotfix should be included (I.E. the tag listed in the last production deployment log for deploy-prod under Github Actions)
2. Create a branch from the tag
3. Implement the hot fix
4. Run `Create Version Tag with Changelog PR' with the hot fix branch updatung the hotfix value in the version by 1 (I.E. 1.8.1 -> 1.8.2)
5. Merge the hot fix branch to the main branch
