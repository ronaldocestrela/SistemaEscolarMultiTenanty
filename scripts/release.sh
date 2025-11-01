#!/bin/bash

# ABCSchool Release Script
# This script helps manage version releases for the ABCSchool project

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Function to print colored output
print_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Function to get current version from version.json
get_current_version() {
    if [ -f "version.json" ]; then
        grep -o '"version": "[^"]*"' version.json | cut -d'"' -f4
    else
        echo "0.0.0"
    fi
}

# Function to update version in version.json
update_version_file() {
    local new_version=$1
    local release_date=$(date +%Y-%m-%d)
    
    cat > version.json << EOF
{
  "version": "$new_version",
  "releaseDate": "$release_date",
  "build": "$new_version.0",
  "preRelease": false,
  "releaseNotes": "Release $new_version of ABCSchool",
  "gitTag": "v$new_version",
  "changelog": {
    "added": [],
    "changed": [],
    "deprecated": [],
    "removed": [],
    "fixed": [],
    "security": []
  },
  "breaking": false,
  "framework": ".NET 9.0",
  "dependencies": {
    "major": [
      "Finbuckle.MultiTenant 9.4.0",
      "Microsoft.EntityFrameworkCore.SqlServer 9.0.9",
      "Microsoft.AspNetCore.Identity.EntityFrameworkCore 9.0.9"
    ]
  }
}
EOF
}

# Function to create git tag
create_git_tag() {
    local version=$1
    local tag="v$version"
    
    print_info "Creating git tag: $tag"
    git tag -a "$tag" -m "Release version $version"
    print_success "Git tag created: $tag"
}

# Function to show help
show_help() {
    echo "ABCSchool Release Script"
    echo ""
    echo "Usage: $0 [command] [options]"
    echo ""
    echo "Commands:"
    echo "  current     Show current version"
    echo "  patch       Create patch release (x.x.X)"
    echo "  minor       Create minor release (x.X.0)"
    echo "  major       Create major release (X.0.0)"
    echo "  tag         Create git tag for current version"
    echo "  help        Show this help message"
    echo ""
    echo "Examples:"
    echo "  $0 current"
    echo "  $0 patch"
    echo "  $0 minor"
    echo "  $0 tag"
}

# Function to increment version
increment_version() {
    local version=$1
    local level=$2
    
    IFS='.' read -ra VERSION_PARTS <<< "$version"
    local major=${VERSION_PARTS[0]}
    local minor=${VERSION_PARTS[1]}
    local patch=${VERSION_PARTS[2]}
    
    case $level in
        "major")
            major=$((major + 1))
            minor=0
            patch=0
            ;;
        "minor")
            minor=$((minor + 1))
            patch=0
            ;;
        "patch")
            patch=$((patch + 1))
            ;;
        *)
            print_error "Invalid version level: $level"
            exit 1
            ;;
    esac
    
    echo "$major.$minor.$patch"
}

# Main script logic
case "${1:-help}" in
    "current")
        current_version=$(get_current_version)
        print_info "Current version: $current_version"
        ;;
    
    "patch"|"minor"|"major")
        current_version=$(get_current_version)
        new_version=$(increment_version "$current_version" "$1")
        
        print_info "Current version: $current_version"
        print_info "New version: $new_version"
        
        read -p "Do you want to create release $new_version? (y/N): " -n 1 -r
        echo
        if [[ $REPLY =~ ^[Yy]$ ]]; then
            update_version_file "$new_version"
            print_success "Version updated to $new_version"
            
            print_info "Don't forget to:"
            print_warning "1. Update CHANGELOG.md with release notes"
            print_warning "2. Commit version changes: git add . && git commit -m 'Release v$new_version'"
            print_warning "3. Create git tag: $0 tag"
            print_warning "4. Push changes: git push && git push --tags"
        else
            print_info "Release cancelled"
        fi
        ;;
    
    "tag")
        current_version=$(get_current_version)
        create_git_tag "$current_version"
        ;;
    
    "help"|*)
        show_help
        ;;
esac