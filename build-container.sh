#!/bin/bash
# ============================================================================
# MorrisonClaudeMCP Docker Build Script
# ============================================================================
# This script helps build, test, and deploy the MCP server container

set -e  # Exit on error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Configuration
IMAGE_NAME="morrison-mcp"
IMAGE_TAG="latest"
CONTAINER_NAME="morrison-mcp-server"

# Functions
print_header() {
    echo -e "${BLUE}========================================${NC}"
    echo -e "${BLUE}$1${NC}"
    echo -e "${BLUE}========================================${NC}"
}

print_success() {
    echo -e "${GREEN}✓ $1${NC}"
}

print_error() {
    echo -e "${RED}✗ $1${NC}"
}

print_warning() {
    echo -e "${YELLOW}⚠ $1${NC}"
}

print_info() {
    echo -e "${BLUE}ℹ $1${NC}"
}

# Check prerequisites
check_prerequisites() {
    print_header "Checking Prerequisites"

    if ! command -v docker &> /dev/null; then
        print_error "Docker is not installed"
        exit 1
    fi
    print_success "Docker is installed"

    if ! command -v docker-compose &> /dev/null; then
        print_warning "docker-compose not found, using 'docker compose' instead"
        DOCKER_COMPOSE="docker compose"
    else
        DOCKER_COMPOSE="docker-compose"
    fi

    if [ ! -f "KateMorrisonMCP.sln" ]; then
        print_error "Solution file not found. Run this script from the project root."
        exit 1
    fi
    print_success "Project files found"

    if [ ! -f "data/canonical_facts.db" ]; then
        print_warning "Database file not found at data/canonical_facts.db"
        print_info "Database will need to be initialized after container build"
    else
        print_success "Database file found"
    fi
}

# Build the Docker image
build_image() {
    print_header "Building Docker Image"

    print_info "Building ${IMAGE_NAME}:${IMAGE_TAG}..."

    docker build \
        --tag ${IMAGE_NAME}:${IMAGE_TAG} \
        --build-arg BUILDKIT_INLINE_CACHE=1 \
        --progress=plain \
        .

    if [ $? -eq 0 ]; then
        print_success "Docker image built successfully"
    else
        print_error "Docker build failed"
        exit 1
    fi
}

# Build using docker-compose
build_compose() {
    print_header "Building with Docker Compose"

    $DOCKER_COMPOSE build

    if [ $? -eq 0 ]; then
        print_success "Docker Compose build successful"
    else
        print_error "Docker Compose build failed"
        exit 1
    fi
}

# Test the container
test_container() {
    print_header "Testing Container"

    print_info "Starting container for testing..."

    # Remove existing test container if it exists
    docker rm -f ${CONTAINER_NAME}-test 2>/dev/null || true

    # Run container in detached mode for testing
    docker run -d \
        --name ${CONTAINER_NAME}-test \
        -v $(pwd)/data:/app/data \
        ${IMAGE_NAME}:${IMAGE_TAG} \
        tail -f /dev/null  # Keep container running for tests

    sleep 2  # Give container time to start

    print_info "Running tests..."

    # Test 1: Check if binary exists
    if docker exec ${CONTAINER_NAME}-test test -f /app/KateMorrisonMCP.Server.dll; then
        print_success "Server binary found"
    else
        print_error "Server binary not found"
        docker rm -f ${CONTAINER_NAME}-test
        exit 1
    fi

    # Test 2: Check if database exists
    if docker exec ${CONTAINER_NAME}-test test -f /app/data/canonical_facts.db; then
        print_success "Database file found"
    else
        print_warning "Database file not found in container"
    fi

    # Test 3: Check database integrity
    if docker exec ${CONTAINER_NAME}-test sqlite3 /app/data/canonical_facts.db "SELECT COUNT(*) FROM characters;" &>/dev/null; then
        CHAR_COUNT=$(docker exec ${CONTAINER_NAME}-test sqlite3 /app/data/canonical_facts.db "SELECT COUNT(*) FROM characters;")
        print_success "Database accessible - ${CHAR_COUNT} characters found"
    else
        print_warning "Could not query database"
    fi

    # Test 4: Check if ingestion tool exists
    if docker exec ${CONTAINER_NAME}-test test -f /app/ingestion/KateMorrisonMCP.Ingestion.dll; then
        print_success "Ingestion tool found"
    else
        print_warning "Ingestion tool not found"
    fi

    # Cleanup test container
    docker rm -f ${CONTAINER_NAME}-test

    print_success "Container tests completed"
}

# Show image info
show_info() {
    print_header "Container Image Information"

    docker images ${IMAGE_NAME}:${IMAGE_TAG}

    print_info "Image layers:"
    docker history ${IMAGE_NAME}:${IMAGE_TAG} --human --no-trunc | head -15
}

# Run the container
run_container() {
    print_header "Running Container"

    print_info "Starting container with docker-compose..."
    $DOCKER_COMPOSE up -d mcp-server

    sleep 2

    print_info "Container status:"
    $DOCKER_COMPOSE ps

    print_info "View logs with: docker-compose logs -f mcp-server"
}

# Stop the container
stop_container() {
    print_header "Stopping Container"

    $DOCKER_COMPOSE down

    print_success "Container stopped"
}

# Clean up
cleanup() {
    print_header "Cleaning Up"

    print_info "Removing containers..."
    $DOCKER_COMPOSE down -v

    print_info "Removing image..."
    docker rmi ${IMAGE_NAME}:${IMAGE_TAG} 2>/dev/null || true

    print_success "Cleanup complete"
}

# Initialize database
init_database() {
    print_header "Initializing Database"

    if [ ! -f "data/create_schema.sql" ] || [ ! -f "data/canonical_facts_data.sql" ]; then
        print_error "Schema or data SQL files not found"
        exit 1
    fi

    print_info "Creating new database..."

    docker run --rm \
        -v $(pwd)/data:/app/data \
        ${IMAGE_NAME}:${IMAGE_TAG} \
        sh -c "cd /app/data && \
               sqlite3 canonical_facts.db < create_schema.sql && \
               sqlite3 canonical_facts.db < canonical_facts_data.sql"

    if [ $? -eq 0 ]; then
        print_success "Database initialized successfully"
    else
        print_error "Database initialization failed"
        exit 1
    fi
}

# Show usage
usage() {
    cat << EOF
Usage: $0 [COMMAND]

Commands:
    build       Build Docker image using Dockerfile
    compose     Build using docker-compose
    test        Run container tests
    info        Show image information
    run         Start container with docker-compose
    stop        Stop running container
    cleanup     Remove containers and images
    init-db     Initialize database from SQL files
    all         Build, test, and show info
    help        Show this help message

Examples:
    $0 build        # Build the Docker image
    $0 test         # Test the built image
    $0 run          # Start the MCP server
    $0 all          # Build and test in one command

EOF
}

# Main script
main() {
    case "${1:-help}" in
        build)
            check_prerequisites
            build_image
            ;;
        compose)
            check_prerequisites
            build_compose
            ;;
        test)
            check_prerequisites
            test_container
            ;;
        info)
            show_info
            ;;
        run)
            check_prerequisites
            run_container
            ;;
        stop)
            stop_container
            ;;
        cleanup)
            cleanup
            ;;
        init-db)
            check_prerequisites
            init_database
            ;;
        all)
            check_prerequisites
            build_image
            test_container
            show_info
            ;;
        help|--help|-h)
            usage
            ;;
        *)
            print_error "Unknown command: $1"
            usage
            exit 1
            ;;
    esac
}

# Run main function
main "$@"
